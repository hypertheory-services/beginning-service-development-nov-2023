# Business API

## Right Now

We are open 9-5 Monday-Friday
    - We open PROMPTLY at 9:00:00
    - Up until 4:59:59

We are closed at 5:00:00

We are closed on the weekends.

We need to let consumers of this API know if we are currently open or closed.

If we are open, the support person is Mitch, and give his email and phone number.

If we are closed, then it should go to our 3rd part support company.

### The Request
```
GET https://api.business.com/status
```

### The Response

#### The response if we are open
```
200 Ok
Content-Type: application/json 

{
    "isOpen": true,
    "supportContact": {
        "name": "Mitch",
        "email": "mitch@company.com",
        "phone": "800 555-1212"
    }

}
```

#### The response if we are closed
```
200 Ok
Content-Type: application/json 

{
    "isOpen": false,
    "supportContact": {
        "name": "Support Pros Inc.",
        "email": "support@support-pros.com",
        "phone": "800 999-1213x23"
    }
}
```
