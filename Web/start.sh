#!/bin/bash

nginx -g "daemon off;" &
dotnet run --project /app/