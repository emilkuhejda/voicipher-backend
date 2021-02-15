using System.Text.Json.Serialization;

namespace Voicipher.Domain.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ErrorCode
    {
        None = 0,

        // Uploaded file not found
        EC100 = 100,

        // File not found
        EC101 = 101,

        // File is not uploaded correctly
        EC102 = 102,

        // File in wrong recognition state
        EC103 = 103,

        // File is not completely uploaded
        EC104 = 104,

        // Recognized audio sample
        EC105 = 105,
        EC106 = 106,
        EC107 = 107,
        EC108 = 108,
        EC109 = 109,
        EC110 = 110,

        // Language not supported
        EC200 = 200,

        // File not supported
        EC201 = 201,

        // Email address is invalid
        EC202 = 202,
        EC203 = 203,
        EC204 = 204,
        EC205 = 205,
        EC206 = 206,
        EC207 = 207,
        EC208 = 208,
        EC209 = 209,
        EC210 = 210,

        // No free minutes in subscription
        EC300 = 300,

        // User has no permissions to purchase registration
        EC301 = 301,

        // Invalid subscription type
        EC302 = 302,

        // Only one file recognition at a time
        EC303 = 303,

        EC304 = 304,
        EC305 = 305,
        EC306 = 306,
        EC307 = 307,
        EC308 = 308,
        EC309 = 309,
        EC310 = 310,

        // Database error
        EC400 = 400,

        EC401 = 401,
        EC402 = 402,
        EC403 = 403,
        EC404 = 404,
        EC405 = 405,
        EC406 = 406,
        EC407 = 407,
        EC408 = 408,
        EC409 = 409,
        EC410 = 410,

        // Migration in progress error
        EC500 = 500,

        EC501 = 501,
        EC502 = 502,
        EC503 = 503,
        EC504 = 504,
        EC505 = 505,
        EC506 = 506,
        EC507 = 507,
        EC508 = 508,
        EC509 = 509,
        EC510 = 510,

        // Invalid input data
        EC600 = 600,

        // Invalid output data
        EC601 = 601,

        // Invalid command result
        EC602 = 602,

        // Operation error
        EC603 = 603,

        EC604 = 604,
        EC605 = 605,
        EC606 = 606,
        EC607 = 607,
        EC608 = 608,
        EC609 = 609,
        EC610 = 610,

        EC700 = 700,
        EC701 = 701,
        EC702 = 702,
        EC703 = 703,
        EC704 = 704,
        EC705 = 705,
        EC706 = 706,
        EC707 = 707,
        EC708 = 708,
        EC709 = 709,
        EC710 = 710,

        // Operation cancelled
        EC800 = 800,

        EC801 = 801,
        EC802 = 802,
        EC803 = 803,
        EC804 = 804,
        EC805 = 805,
        EC806 = 806,
        EC807 = 807,
        EC808 = 808,
        EC809 = 809,
        EC810 = 810,

        // Unauthorized
        Unauthorized = 900
    }
}
