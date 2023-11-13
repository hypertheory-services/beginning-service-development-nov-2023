# We will start with our trusty ubuntu:20.04 but label it as 'build' since we will only use this to build our application
FROM ubuntu:20.04 as build
# Each RUN creates a new layer, no matter how many commands you 'chain' together.
# This one will run apt-get install AFTER apt-get update completes.
RUN apt-get update && apt-get install -y build-essential
WORKDIR /app/
COPY src/hello.c /app/
## Added the '-static' here to staticly link the stdio library - dependency free executable.
RUN gcc -o hello -static hello.c
## Another FROM command - this time from Alpine - it's tiny. Like 5 mb. 
FROM scratch as final
WORKDIR /app/
COPY --from=build /app/hello ./
ENTRYPOINT [ "/app/hello" ]