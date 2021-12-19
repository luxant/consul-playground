#!/bin/sh

consul agent -config-dir=/consul/config/ &
dotnet app/Web.dll --urls http://0.0.0.0:5000
