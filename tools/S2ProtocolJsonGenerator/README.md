# S2ProtocolJsonGenerator

Regenerates the compact runtime protocol JSON files embedded by `s2protocol.NET`.

Run from the repository root:

```powershell
dotnet run --project tools/S2ProtocolJsonGenerator/S2ProtocolJsonGenerator.csproj
```

The tool reads its embedded Blizzard protocol version files from
`tools/S2ProtocolJsonGenerator/Resources/versions/protocol*.py` and writes
matching `protocol*.json` files to `src/s2protocol.NET/Resources/versions`.
The library embeds only the JSON files at runtime.
