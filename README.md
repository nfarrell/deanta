# Deanta?
Irish translation of "Done".

Roughly implemented in:
Simple web app - https://github.com/nfarrell/deanta/tree/master
ES version vaguely implemented in CosmosDB with SQL backing - https://github.com/nfarrell/deanta/tree/cosmos-example
CQRS (Query Command segregation) - https://github.com/nfarrell/deanta/tree/microservices-example

The simple web app, which actually utilises the SSO (single sign-on) built on IdentityServer4 and Skoruba is built and deployed to here: https://deanta.azurewebsites.net/Identity/Account/Login

Cloudflare is implemented using a custom domain I purchased through Blacknight at this URL -  https://deanta.me/ . I use Cloudflare's simple SSL hosting. Unfortunately, Azure takes too long and too much hassle to implement custom SSL - even though I have done it before... e.g. id.boafy.com uses Azure's SSL.
