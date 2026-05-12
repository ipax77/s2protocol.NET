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

The generator intentionally uses the Python protocol files rather than the JSON
files from Blizzard's `s2protocol` repository. The Python files already contain
the compact numeric decoder tables used by the original runtime, which map
directly to `s2protocol.NET`'s decoder model. Blizzard's JSON files describe a
richer protocol schema, so using them would require an additional schema-to-
decoder-table translation step before generating the runtime JSON.
