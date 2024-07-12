using Domain.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PCM_LavoroAgile.Controllers
{
    /// <summary>
    /// Classe per esporre dei servizi in BasicAuthentication
    /// Viene utilizzare un ActionFilterAttribute
    /// Di seguito un esempio
    /// </summary>
    [AllowAnonymous]
    public class WebServicesBasicAuthentication : Controller
    {
        private readonly ILogger<AccordiController> _logger;
        private readonly IDocumentsService _documentsService;
        private readonly IAccordoService _accordoService;
        private readonly IRepository<Accordo, Guid> _repository;

        public WebServicesBasicAuthentication(ILogger<AccordiController> logger, IDocumentsService documentsService, IRepository<Accordo, Guid> repository, IAccordoService accordoService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _documentsService = documentsService ?? throw new ArgumentNullException(nameof(documentsService));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _accordoService = accordoService ?? throw new ArgumentNullException(nameof(accordoService));
        }

        /// <summary>
        /// Esempio di un servizio in BasicAuthentication
        /// </summary>
        /// <param name="data"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        //[HttpGet]
        //[BasicAuthenticationAttribute("usr.administrator@pcm.governo.it", "P@ssWord.1!")]
        //public async Task<ActionResult> WebService(string param1, string param2, CancellationToken cancellationToken)
        //{
        //    return null;
        //}        
    }

    public class BasicAuthenticationAttribute : ActionFilterAttribute
    {
        protected string Username { get; set; }
        protected string Password { get; set; }

        public BasicAuthenticationAttribute(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var req = filterContext.HttpContext.Request;
            if (!String.IsNullOrEmpty(req.Headers["Authorization"]))
            {
                var auth = AuthenticationHeaderValue.Parse(req.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(auth.Parameter);
                var cred = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                var user = new { Name = cred[0], Pass = cred[1] };
                if (user.Name == Username && user.Pass == Password)
                    return;
            }
            filterContext.Result = new JsonResult(new { HttpStatusCode.Unauthorized });           
        }
    }
}
