# AdamServer

## The order of service implementation

1. Execute shell command (python, bash)
2. Service managment

## Interaction of processes and services

### Execute shell command

[simple web api] => [base auth] => [command] =>  [execute python] => [websocket?] => [execute result]

### Service managment

[simple web api] => [base auth] => [command] => [execute bash (Linux) || execute cmd (Windows)] => [execute result] => [simple web api]