# MCBA

s3947204

Ritish Kandel

https://github.com/rmit-wdt-summer-2024/s3947204-a2


## API endpionts

Endpoint: api/Customer

Description: It returns a list of customers in JSON format


Endpoint: api/Customer/{id}

Description: It returns a customer that matches the given id


Endpoint: api/Lock/{id}

Description: It changes the lock field of the Customer entity that matches the given id to be true


Endpoint: api/UnLock/{id}

Description: It changes the lock field of the Customer entity that matches the given id to be false


## References

- The code for setting up the dependency injection in MCBA.Tests was heavily adapted from week 10 lectorial
- The extension methods in the Utilities class library was also heavily adapted from day 6 lab, project McbaExampleWithLogin
- The home page image was generated using Bing image generator: 
    link: https://www.bing.com/images/create/a-wide-landscape-picture-that-has-a-white-building/1-65ac66a5ea3e4ee7980e14be298460b6?id=%2bZg%2bh1%2fgPQ8jI8A6msKPBw%3d%3d&view=detailv2&idpp=genimg&FORM=GCRIDP&ajaxhist=0&ajaxserp=0
- To mock the session for unit testing, the code was adapted from stackover flow question: https://stackoverflow.com/questions/47203333/how-to-mock-session-object-in-asp-net-core
