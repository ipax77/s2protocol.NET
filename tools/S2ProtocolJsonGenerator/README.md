# S2ProtocolJsonGenerator

Regenerates the compact runtime protocol JSON files embedded by `s2protocol.NET`.

Run from the repository root:

```powershell
dotnet run --project tools/S2ProtocolJsonGenerator/S2ProtocolJsonGenerator.csproj
```

The tool reads its embedded Blizzard protocol version files from
`tools/S2ProtocolJsonGenerator/Resources/versions/protocol*.py`. It writes one
full `protocol*.json` payload for each unique protocol schema and a compact
`protocol-versions.json` manifest mapping every supported build to its payload.
The earliest build is used as the canonical payload when multiple builds have
identical generated content.

The generated output is deterministic. Files from an older generation that are
no longer canonical are removed. With the current 95 Blizzard versions, this
reduces the embedded protocol data from about 2.59 MB to about 560 KB while
preserving every supported build number.

The library embeds only these generated JSON files at runtime. Aliased builds
share their deserialized decoder tables in memory and retain their own reported
build number.

The generator intentionally uses the Python protocol files rather than the JSON
files from Blizzard's `s2protocol` repository. The Python files already contain
the compact numeric decoder tables used by the original runtime, which map
directly to `s2protocol.NET`'s decoder model. Blizzard's JSON files describe a
richer protocol schema, so using them would require an additional schema-to-
decoder-table translation step before generating the runtime JSON.
