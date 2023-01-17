# SafeShareDotnet

Simple pet app for sharing files between computers on the same network.

The main goal of the project is to allow people to safely share files and/or text.

**The app is written in C# and uses .NET Core 7.**

## Features

- [x]  Users can **register** (using email + password) and then **log in** using credentials specified when registering.
- [x]  Authorized users have the ability to:
    - [x]  Upload a file
    - [x]  Upload a text (string)
    - [x]  Specify whether the resource should be deleted after it is viewed (for both messages and files).
    - [x]  Get the list of files/texts they have uploaded previously. Users can't see deleted files/texts.
    - [x]  Delete a file/text they have uploaded earlier.
      <aside>
      💡 The resource urls are based on hashes, so they are not guessable, therefore "protected" from guessing.
      </aside>
- [x]  Anonymous users can access files/texts using the generated URLs.

## Additional features

- [ ]  Tests
- [x]  Docker-compose file for the application infrastructure
- [x]  External blob storage for files

## Development

1. Create `.env` based on `.env.template` and fill in the values.
2. Run the app via docker-compose:
    ```shell
    docker-compose up
    ```
3. Go to <http://localhost:8080/swagger/index.html> to see the API documentation.