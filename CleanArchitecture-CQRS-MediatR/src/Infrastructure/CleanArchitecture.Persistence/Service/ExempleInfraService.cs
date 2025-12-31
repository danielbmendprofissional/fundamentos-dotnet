namespace CleanArchitecture.Persistence.Service
{
    internal class ExempleInfraService
    {
        // This is where external services would be placed (calls to external services)

        /*   
            ABOUT WHERE THE INTERFACE FOR AN EXTERNAL SERVICE SHOULD BE:

            - Application: when the service is something "extra" to the business rule,
              such as sending an email after completing a registration.

            - Domain: when the business rule depends on the service in order to exist.
        */
    }
}