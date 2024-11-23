# AdamServer

## The order of service implementation

1. Execute shell command (python, bash)
2. Service managment

## Interaction of processes and services

### Execute shell command

[incoming command] => [web socket? || simple web api] =>  [execute python] => [websocket?] => [execute result]

### Service managment

[simple web api] => [incoming command] => [execute bash] => [execute result] => [simple web api]