FROM ubuntu:20.04
RUN apt-get update
RUN apt-get install -y build-essential
WORKDIR /app
COPY src/hello.c /app/
RUN gcc -o hello hello.c
ENTRYPOINT ["/app/hello"]
