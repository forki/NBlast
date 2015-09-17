﻿using System.Collections.Generic;
using System.Web.Http;
using NBlast.Rest.Configuration;
using Ninject.Infrastructure.Language;

namespace NBlast.Rest.Controllers.Api
{
    [RoutePrefix("api/values")]
    public class ValuesController: ApiController
    {

        // GET api/values 
        public IEnumerable<string> Get()
        {
            return new[] { "value1", "value2" }.ToEnumerable();
        }

        // GET api/values/5 
        public string Get(int id)
        {
            return "value";
        }
    }
}