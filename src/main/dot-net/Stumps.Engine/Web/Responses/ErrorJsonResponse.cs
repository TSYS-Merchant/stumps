namespace Stumps.Web.Responses
{

    using System;
    using Nancy;
    using Nancy.Responses;
    using Stumps.Web.Models;

    public class ErrorJsonResponse : JsonResponse
    {

        private readonly ErrorModel _errorModel;

        private ErrorJsonResponse(ErrorModel error) : base(error, new DefaultJsonSerializer())
        {

            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            _errorModel = error;

        }

        public string ErrorMessage
        {
            get { return _errorModel.ErrorMessage; }
        }

        public string FullException
        {
            get { return _errorModel.FullException; }
        }

        public static ErrorJsonResponse FromMessage(string message)
        {

            var error = new ErrorModel
            {
                ErrorMessage = message
            };

            var response = new ErrorJsonResponse(error);

            return response;

        }

        public static ErrorJsonResponse FromException(Exception ex)
        {

            if (ex == null)
            {
                return new ErrorJsonResponse(
                    new ErrorModel
                    {
                        ErrorMessage = null,
                        Errors = null,
                        FullException = null
                    });
            }

            var rootException = ex.GetBaseException();

            var error = new ErrorModel
            {
                ErrorMessage = Resources.ErrorUnexpected + rootException.Message,
                FullException = rootException.ToString()
            };

            ErrorJsonResponse response = null;

            try
            {
                response = new ErrorJsonResponse(error)
                {
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
            catch
            {
                if (response != null)
                {
                    response.Dispose();
                }

                throw;
            }

            return response;

        }

    }

}