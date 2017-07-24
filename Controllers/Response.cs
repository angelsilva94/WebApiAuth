using System.Collections.Generic;
using System.Net;

namespace ApiAuth.Controllers {
    internal class Response {
        private HttpStatusCode _code;

        public Response (HttpStatusCode code) {
            this._code = code;
        }

        public string Message { get; set; }
        public IEnumerable<Response> Errors { set; get; }
    }
}