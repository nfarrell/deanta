# Deanta?
Irish translation of "Done".

Roughly implemented in:
Simple web app - https://github.com/nfarrell/deanta/tree/master
ES version vaguely implemented in CosmosDB with SQL backing - https://github.com/nfarrell/deanta/tree/cosmos-example
CQRS (Query Command segregation) - https://github.com/nfarrell/deanta/tree/microservices-example

The simple web app, which actually utilises the SSO (single sign-on) built on IdentityServer4 and Skoruba is built and deployed to here: https://deanta.azurewebsites.net/Identity/Account/Login

OpenIDConnect is a login option on the website, and it uses https://id.boafy.com as its authority and is manageable through https://admin.boafy.com.

Cloudflare is implemented using a custom domain I purchased through Blacknight at this URL -  https://deanta.me/ . I use Cloudflare's simple SSL hosting. Unfortunately, Azure takes too long and too much hassle to implement custom SSL - even though I have done it before... e.g. id.boafy.com uses Azure's SSL.

Analytics can be viewed at: https://stats.boafy.com/dashboard username is deloitte and password is Deloitte_123 . The reason I include this that it's hosted on its own Azure VM running Linux Ubuntu 18.04 with it's own IP, firewalls and NSG.
