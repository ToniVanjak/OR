using Microsoft.AspNetCore.Mvc;

namespace labos2.Models
{
    public class WrapperResponse
    {
        public static IActionResult OkResponse(string message, List<Igrac> data)
        {
            var apiResponse = new Wrapper
            {
                Status = "OK",
                Message = message,
                ResponseData = data
            };
            return new ObjectResult(apiResponse)
            {
                StatusCode = StatusCodes.Status200OK
            };
        }

        public static IActionResult NotFoundResponse(string message)
        {
            var apiResponse = new Wrapper
            {
                Status = "Not Found",
                Message = message,
                ResponseData = null
            };
            return new ObjectResult(apiResponse)
            {
                StatusCode = StatusCodes.Status404NotFound
            };
        }

        public static IActionResult BadRequestResponse(string message)
        {
            var apiResponse = new Wrapper
            {
                Status = "Bad Request",
                Message = message,
                ResponseData = null
            };
            return new ObjectResult(apiResponse)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }


        public static IActionResult OkPutResponse(string message)
        {
            var apiResponse = new Wrapper
            {
                Status = "OK",
                Message = message,
                ResponseData = null
            };
            return new ObjectResult(apiResponse)
            {
                StatusCode = StatusCodes.Status200OK
            };
        }
        public static IActionResult NotImplementedResponse()
        {
            var apiResponse = new Wrapper
            {
                Status = "Not Implemented",
                Message = "Method not implemented for requested resource.",
                ResponseData = null
            };
            return new ObjectResult(apiResponse)
            {
                StatusCode = StatusCodes.Status501NotImplemented
            };
        }

        public static IActionResult CreatedResponse()
        {
            var apiResponse = new Wrapper
            {
                Status = "Created",
                Message = "Successfully created and added to the database",
                ResponseData = null
            };
            return new ObjectResult(apiResponse)
            {
                StatusCode = StatusCodes.Status201Created
            };
        }

    }
}
