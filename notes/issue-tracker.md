# Issue Tracker API

- Sprint 1

We need a way for users to log issues for a particular piece of supported software. 
The issue should contain a priority (Question, Bug, Feature Request, or High Priority)
A brief description of the issue
The date and time the issue was logged
The name of the person filing the issue.

## Command
    Log An Issue
        - Software
        - Priority
        - Description
        - DAte and Time The Issue Was Logged
        - The Identity of the person filing the issue.


## Http - Resource

/issues

## Http Methods

POST 

## Http - Representations (the data coming from or going to the API)

```json
{
    "softwareId": "excel",
    "priority": "Bug",
    "description": "Won't let me add more than 65k rows",
    "when": "ISO8601",
    "who": "Joe Schmidt"
}

```

