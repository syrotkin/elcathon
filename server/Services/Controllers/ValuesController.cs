using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Newtonsoft.Json;
using System.Web;
using System.Web.Http.Cors;
using Services.Models;
using Services.Controllers.Messages;

namespace Services.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ValuesController : ApiController
    {
        // GET api/values
        [HttpGet]
        [Route("api/values")]
        public IEnumerable<string> Get()
        {
            var output = HttpUtility.UrlEncode("{sensorId: 1, sensorType: \"temperature\", value: 21}");

            return new string[] { DateTime.Now.ToString() };
        }

        [HttpGet]
        [Route("api/adduser")]
        public IHttpActionResult AddUser(string user)
        {
            var testAddUserMessage = new AddUserMessage { Username = "bob", PasswordHash = "abcd" };
            var encoded = UrlEncode(testAddUserMessage);

            var decodedInput = HttpUtility.UrlDecode(user);
            var addUserMessage = JsonConvert.DeserializeObject<AddUserMessage>(decodedInput);

            int assignedId;
            using (var context = new FirstSampleContext()) {
                var insertedUser = new SensorUser
                {
                    Username = addUserMessage.Username,
                    PasswordHash = addUserMessage.PasswordHash,
                    Score = 0
                };

                context.SensorUserSet.Add(insertedUser);
                context.SaveChanges();

                assignedId = insertedUser.UserId;
            }
            var response = new AddUserResponse { UserId = assignedId };
            return Ok(response);
        }

        [HttpGet]
        [Route("api/addsensor")]
        public IHttpActionResult AddSensor(string sensor)
        {
            var testAddSensorMessage = new AddSensorMessage { UserId = 1, SensorType = "humidity" };
            var encoded = UrlEncode(testAddSensorMessage);

            var decodedInput = HttpUtility.UrlDecode(sensor);
            var sensorTypeMessage = JsonConvert.DeserializeObject<AddSensorMessage>(decodedInput);

            int assignedId;
            using (var context = new FirstSampleContext())
            {
                var newSensor = new Sensor
                {
                    SensorType = sensorTypeMessage.SensorType,
                    UserId = sensorTypeMessage.UserId
                };
                context.SensorSet.Add(newSensor);
                context.SaveChanges();
                assignedId = newSensor.SensorId;
            }

            var response = new AddSensorResponse { SensorId = assignedId };
            return Ok(response);
        }

        private static string UrlEncode(object message)
        {
            var serializedMessage = JsonConvert.SerializeObject(message);
            var encodedMessage = HttpUtility.UrlEncode(serializedMessage);
            return encodedMessage;
        }
        
        // GET api/sensordata
        [HttpGet]
        [Route("api/sensordata")]
        public IHttpActionResult StoreSensorData(string data)
        {
            var testStoreMessage = new SensorDataMessage { SensorId = 3, SensorType = "binary", Value = 0 };
            var encoded = UrlEncode(testStoreMessage);


            var decodedInput = HttpUtility.UrlDecode(data);
            var sensorDataMessage = JsonConvert.DeserializeObject<SensorDataMessage>(decodedInput);

            var now = DateTime.Now;

            int newSensorDataId;
            using (var context = new FirstSampleContext())
            {
                var newSensorData = new SensorData
                {
                    SensorId = sensorDataMessage.SensorId,
                    Value = sensorDataMessage.Value,
                    Received = now
                };
                context.SensorDataSet.Add(newSensorData);
                context.SaveChanges();
                newSensorDataId = newSensorData.SensorDataId;
               // SetChallengeStatusIfRequired(sensorDataMessage, context);

            }
            return Ok(newSensorDataId);
        }

        /// <summary>
        /// // TODO: does not work, needs more work.
        /// Sets challenge Status to "failed" or "succeeded" if necessary
        /// </summary>
        /// <param name="sensorDataMessage"></param>
        /// <param name="context"></param>
        private static void SetChallengeStatusIfRequired(SensorDataMessage sensorDataMessage, FirstSampleContext context)
        {
            // based on the sensor data update the challenge table
            /**
                select * from challenge c
                inner join sensor S
                on s.UserId = c.victimId 
                where 
                s.SensorId = c.SensorId
                and c.Status = 'accepted'
                and s.SensorId = 3
              */
            var relevantChallenges = context.ChallengeSet
                .Join(context.SensorSet,
                c => c.VictimId,
                s => s.UserId,
                (c, s) => new { c, s }).
                Where(cs => cs.s.SensorId == cs.c.SensorId
                && cs.c.VictimId == cs.s.UserId
                && cs.c.Status == "accepted"
                && cs.s.SensorId == sensorDataMessage.SensorId)
                .Select(cs => new 
                {
                    ChallengeId = cs.c.ChallengeId,
                    ChallengeTypeId = cs.c.ChallengeTypeId,
                    StartDate = cs.c.StartDate
                });

            foreach (var relevantChallenge in relevantChallenges)
            {
                var existingChallenge = context.ChallengeSet.Where(c => c.ChallengeId == relevantChallenge.ChallengeId)
                    .Join(context.ChallengeTypeSet, c => c.ChallengeTypeId, ct => ct.ChallengeTypeId, (c, ct) => new { c, ct })
                    .Where(cct => DateTime.Now - cct.c.StartDate < new TimeSpan(0, 0, cct.ct.DurationSeconds)).Select(cct => cct.c).FirstOrDefault();
                if (existingChallenge != null)
                {
                    existingChallenge.Status = "succeeded";
                    context.SaveChanges();
                }
            }
            
            var query = relevantChallenges
                .Join(context.ChallengeTypeSet,
                rc => rc.ChallengeTypeId,
                ct => ct.ChallengeTypeId,
                (rc, ct) => new { rc.ChallengeId, rc.StartDate, ct.DurationSeconds })
                .Where(rcct => DateTime.Now - rcct.StartDate > new TimeSpan(0, 0, rcct.DurationSeconds));
            var succeededChallenges = query.ToList();
            foreach (var succeededChallenge in succeededChallenges)
            {
                var existing = context.ChallengeSet.Where(c => c.ChallengeId == succeededChallenge.ChallengeId).FirstOrDefault();
                if (existing != null)
                {
                    existing.Status = "succeeded";
                    context.SaveChanges();
                }
            }


            /*
         select Getdate() - StartDate as currentDuration, DurationSeconds, ct.Type
        from challenge c
        inner join challengeType ct
        on ct.ChallengeTypeId  = c.ChallengeTypeId
        where challengeId = 1
        -- given this result  --> if currentDuration > DurationSeconds
        -- update challenge set status = 'succeeded' where challengeId = 1
        -- hardcode type logic:
        if (notv)
        -- if dateTime.Now - StartDate > DurationInSeconds --> succeeded
        -- if the current!!! data point == 1 --> fail    
         */
        }

        [HttpGet]
        [Route("api/sensordata")]
        public IHttpActionResult GetSensorData(int sensorId)
        {
            using (var context = new FirstSampleContext())
            {
                var queryResult = context.SensorDataSet
                    .Join(context.SensorSet, 
                    sd => sd.SensorId, 
                    s => s.SensorId, 
                    (sd, s) => new { sd, s })
                    .Where(sds => sds.sd.SensorId == sensorId)
                    .OrderByDescending(sds => sds.sd.Received).Take(20)
                    .ToList();

                if (queryResult.Count == 0) {
                    return NotFound();
                }

                var response = new
                {
                    SensorId = queryResult.First().s.SensorId,
                    SensorType = queryResult.First().s.SensorType,
                    Values = queryResult.Select(sd => sd.sd.Value).ToArray()
                };

                return Ok(response);
            }
        }

        [HttpGet]
        [Route("api/getfriends")]
        public IHttpActionResult GetFriends(int userId) {
            using (var context = new FirstSampleContext())
            {
                var result = context.SensorUserSet.Where(u => u.UserId != userId).Take(2).ToList();
                if (result.Count == 0)
                {
                    return NotFound();
                }
                var response = result.Select(u => new { u.UserId, u.Username });
                return Ok(response);
            }
        }

        [HttpGet]
        [Route("api/getChallengeTypes")]
        public IHttpActionResult GetChallengeTypes()
        {
            using (var context = new FirstSampleContext())
            {
                var result = context.ChallengeTypeSet.ToList();
                return Ok(result);
            }
        }

        [HttpGet]
        [Route("api/createChallenge")]
        public IHttpActionResult CreateChallenge(string data)
        {
            var testCreateChallengeMessage = new CreateChallengeMessage { OwnerId = 2, VictimId = 1, ChallengeType = "freeze" };
            var encoded = UrlEncode(testCreateChallengeMessage);
            
            var decodedInput = HttpUtility.UrlDecode(data);
            var message = JsonConvert.DeserializeObject<CreateChallengeMessage>(decodedInput);

            var ownerId = message.OwnerId;
            var victimId = message.VictimId;
            var challengeType = message.ChallengeType;

            // need to check if the victim has a sensor of the type required by the challenge
            using (var context = new FirstSampleContext())
            {
                var result = context.SensorSet.Join(context.ChallengeTypeSet, 
                    s => s.SensorType, 
                    ct => ct.SensorType, 
                    (s, ct) => new { s, ct}).
                    Where(sct => sct.ct.Type == challengeType && sct.s.UserId == victimId)
                    .ToList();
                CreateChallengeResponse response = new CreateChallengeResponse { Status = "impossible" };
                if (result.Count == 0)
                {
                    return Ok(response);
                }
                var sensorId = result[0].s.SensorId;
                var challengeTypeId = result[0].ct.ChallengeTypeId;
                var newChallenge = new Challenge
                {
                    ChallengeTypeId = challengeTypeId,
                    OwnerId = ownerId,
                    VictimId = victimId,
                    SensorId = sensorId,
                    StartDate = DateTime.Now,
                    Status = "created"
                };

                context.ChallengeSet.Add(newChallenge);
                context.SaveChanges();
                response = new CreateChallengeResponse { Status = "created" };
                return Ok(response);
            }
        }

        [HttpGet]
        [Route("api/getChallenges")]
        public IHttpActionResult GetChallenges(int? ownerId = null, int? victimId = null)
        {      
            var result = Enumerable.Empty<Challenge>().ToList();
            using (var context = new FirstSampleContext())
            {
                if (ownerId != null)
                {
                    if (victimId != null)
                    {
                        result = context.ChallengeSet.Where(c => c.OwnerId == ownerId && c.VictimId == victimId).ToList();
                    }
                    else
                    {
                        result = context.ChallengeSet.Where(c => c.OwnerId == ownerId).ToList();
                    }
                }
                else
                {
                    if (victimId != null)
                    {
                        result = context.ChallengeSet.Where(c => c.VictimId == victimId).ToList();
                    }
                }
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("api/acceptChallenge")]
        public IHttpActionResult AcceptChallenge(int challengeId)
        {
            using (var context = new FirstSampleContext())
            {
                var existingChallenge = context.ChallengeSet.Where(c => c.ChallengeId == challengeId).FirstOrDefault();
                if (existingChallenge == null)
                {
                    return BadRequest($"Challenge with ID = {challengeId} does not exist.");
                }
                existingChallenge.Status = "accepted";
                context.SaveChanges();
                return Ok();
            }
        }
                          
        // POST api/test
        [HttpPost]
        [Route("api/test")]
        public string PostTest([FromBody] string input)
        {
            //var result = $"sensorId: {input.sensorId}, temperature: {input.value.temperature}";
            //return result;

            //var o = JObject.Parse(input);

            // var parsedObject = JsonConvert.DeserializeObject()
            //var serializer = new JsonSerializer();
            //var reader = new StringReader(input);
            //var o = (JObject)serializer.Deserialize(reader);

            var parsedMessage = JsonConvert.DeserializeObject<SensorDataMessage>(input);

            var result = $"sensorId: {parsedMessage.SensorId}, type: {parsedMessage.SensorType}, value: {parsedMessage.Value}";
            return result;
        }

    }
}
