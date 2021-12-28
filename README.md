# Building the images

## Server image

`docker build -t consul-server -f Dockerfile-server .`

## Client image

`docker build -t consul-client -f Dockerfile-client .`


# UI consul interface

Open your browser and go to http://localhost:8500/

# REST api

## Query nodes

`curl localhost:8500/v1/catalog/nodes`

## Get key as JSON

`curl http://localhost:8500/v1/kv/appsettings/qa4/web`

## Get key as JSON printed formatted

`curl http://localhost:8500/v1/kv/appsettings/qa4/web | json_pp`

# Running the project

1. Run `docker compose up` so the services and containers are up and running
1. Publish the ASP.NET application to `C:\tmp` according to the volumne map on `compose.yml` (`- /c/tmp:/app`)
1. Go to `localhost` on your browser