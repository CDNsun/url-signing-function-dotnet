#Token generator for URL Signing

REQUIRES
 * .NET Framework 3.5+

BUILD

  To build the generator open the solution in [MS Visual Studio][msvs] and build the project in Release Mode. Upon success of the build, you will find the exe file (**UrlSigningFunction.exe**) in the folder named ***UrlSigningFunction\bin\Release***

USAGE
```
UrlSigningFunction.exe -s <scheme> -r <CDN domain> -p <file path> -k <URL Signing Key> [-e <expiration time> ] [-i <IP>]
```

PARAMETERS

* scheme  
  * http or https, default = http
* CDN domain
  * CDN service domain
  * example: '12345.r.cdnsun.net'
* file path
  * e.g. '/images/photo.jpeg'
* URL Signing Key
  * URL Signing Key from the https://cdnsun.com/cdn/settings page
  * example: 'jfXNDdkOp2'
* expiration time (optional)
  * expiration time of token, UNIX timestamp format.
  * example: 1333497600
* IP (optional)
  * Allow access only to the specified IP address
  * example: '1.2.3.4'


TO GENERATE TOKEN
```
UrlSigningFunction.exe -s 'http' -r '12345.r.cdnsun.net' -p '/author/david007/page/3/www.jwplayer.com' -k 'jfXNDdkOp2' -e 1333497600 -i '1.2.3.4'
```
Sample Output:
```
http://12345.r.cdnsun.net/author/david007/page/3/www.jwplayer.com?secure=ZTNhNjYyNjE3OTA3ZDUyYTRlOWQzMzRmMTEyZDg5NDI&expires=1333497600&ip=1.2.3.4
```
[msvs]:https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx
  
