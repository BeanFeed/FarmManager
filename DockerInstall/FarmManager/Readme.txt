Setup Farm Manager

General Setup:

In docker-compose.yml

Set Cors__Origin value under environment to localhost or local ip address of the machine as http://{ip address}:{Port Number}
Make sure you have the Port number set to the same value as FRONTEND_PORT.

Set Jwt__Issuer and Jwt__Audience to the same value as Cors__Origin.

Proxy Setup:

Set Cors__Origin value under environment to the hostname of the machine as https://{DomainName}

Set Jwt__Issuer to the same value as Cors__Origin.

Set Jwt__Audience to https://api.{DomainName}

Set Jwt__Secure and UseHttpsRedirection to true.

Make sure that the domain pointing to the backend is api.{DomainName}
