Web server test suite
=====================

## Requirements ##

* Respond to `GET` with status code in `{200,404,403}`
* Respond to `HEAD` with status code in `{200,404,403}`
* Respond to all other request methods with status code `405`
* Directory index file name `index.html`
* Respond to requests for `/<file>.html` with the contents of `DOCUMENT_ROOT/<file>.html`
* Requests for `/<directory>/` should be interpreted as requests for `DOCUMENT_ROOT/<directory>/index.html`
* Respond with the following header fields for all requests:
  * `Server`
  * `Date`
  * `Connection`
* Respond with the following additional header fields for all `200` responses to `GET` and `HEAD` requests:
  * `Content-Length`
  * `Content-Type`
* Respond with correct `Content-Type` for `.html, .css, js, jpg, .jpeg, .png, .gif, .swf`
* Respond to percent-encoding URLs
* Correctly serve a 2GB+ files
* No security vulnerabilities

## Testing environment ##

* Put `Dockerfile` to web server repository root
* Prepare docker container to run tests:
  * Read config file `/etc/httpd.conf`
  * Expose port 80

Config file spec:
```
cpu_limit 4       # maximum CPU count to use (for non-blocking servers)
thread_limit 256  # maximum simultaneous connections (for blocking servers)
document_root /var/www/html
```

Run tests:
```
git clone https://github.com/init/http-test-suite.git
cd http-test-suite

docker build -t bykov-httpd https://github.com/init/httpd.git
docker run -p 80:80 -v /etc/httpd.conf:/etc/httpd.conf:ro -v /var/www/html:/var/www/html:ro --name bykov-httpd -t bykov-httpd

./httptest.py
```

# ab тест http-server ab -n 10000 -c 8  -r http://127.0.0.1:1234/httptest/wikipedia_russia.html
```
Concurrency Level:      8
Time taken for tests:   7.668 seconds
Complete requests:      10000
Failed requests:        0
Total transferred:      9549740000 bytes
HTML transferred:       9548240000 bytes
Requests per second:    1304.19 [#/sec] (mean)
Time per request:       6.134 [ms] (mean)
Time per request:       0.767 [ms] (mean, across all concurrent requests)
Transfer rate:          1216277.28 [Kbytes/sec] received

Connection Times (ms)
              min  mean[+/-sd] median   max
Connect:        0    1   0.5      1      16
Processing:     3    5   1.2      5      27
Waiting:        0    1   0.5      1      21
Total:          3    6   1.3      6      28

Percentage of the requests served within a certain time (ms)
  50%      6
  66%      6
  75%      6
  80%      6
  90%      7
  95%      8
  98%      9
  99%     10
 100%     28 (longest request)
```

# ab тест ngnix ab -n 10000 -c 8 http://127.0.0.1/httptest/wikipedia_russia.html
```
Server Software:        nginx/1.19.5
Server Hostname:        127.0.0.1
Server Port:            80

Document Path:          /httptest/wikipedia_russia.html
Document Length:        954824 bytes

Concurrency Level:      8
Time taken for tests:   0.579 seconds
Complete requests:      1000
Failed requests:        0
Total transferred:      955062000 bytes
HTML transferred:       954824000 bytes
Requests per second:    1725.71 [#/sec] (mean)
Time per request:       4.636 [ms] (mean)
Time per request:       0.579 [ms] (mean, across all concurrent requests)
Transfer rate:          1609527.51 [Kbytes/sec] received

Connection Times (ms)
              min  mean[+/-sd] median   max
Connect:        0    0   0.2      0       2
Processing:     2    4   1.2      4      10
Waiting:        0    1   0.7      1       4
Total:          2    5   1.3      4      10

Percentage of the requests served within a certain time (ms)
  50%      4
  66%      4
  75%      5
  80%      6
  90%      6
  95%      8
  98%      9
  99%      9
 100%     10 (longest request)
```

Разница в RPS ~= 1,32 

# Количество RPS в зависимости от количества потоков 
```
1 поток - 584.25 RPS
2 потока - 824.99 RPS
4 потока - 1089.45 RPS
8 потоков - 1423.13 RPS
16 потоков - 1518.72 RPS
32 потока - 1606.06 RPS
```
# Запуск через dotnet

```
dotnet build
dotnet run  <Путь к config файлу>
```

# Запуск через docker

```
docker build -t server -f Dockerfile . 
docker run -p 80:<Порт из config> server <Путь к config файлу> (
docker run -p 80:1234  server  config.config)
```
