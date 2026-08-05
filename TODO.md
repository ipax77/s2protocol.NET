# v1.0 release checklist

The decoder is feature-complete for the currently bundled StarCraft II protocol surface. Before publishing v1.0, freeze new features and complete the correctness, security, API, and release-hardening work below.

## Release blockers

- [x] Support bit-packed bit arrays larger than 64 bits without truncation.
  - `BitPackedBuffer.ReadBits` currently retains at most 64 bits, although typed models expose `BigInteger` values and protocol lengths can exceed 64 bits.
  - Cover zero, 64-bit, greater-than-64-bit, and high-bit-set values with synthetic tests.
  - Remove the `No BigInteger support` limitation from the README after verification.

- [x] Guarantee archive disposal on success, failure, and cancellation.
  - Use deterministic disposal around both `ReplayDecoder.DecodeAsync` overloads.
  - Decide and document whether `DecodeAsync(Stream)` leaves the caller-owned stream open.
  - Preserve the original exception as the `InnerException` of `DecodeException`.
  - Replace the missing-file `ArgumentNullException` with an appropriate argument or file exception.
  - Add file-handle, stream-ownership, exception, and cancellation tests.

- [x] Harden MPQ handling for untrusted or malformed archives.
  - Reject rooted paths and traversal segments before extracting archive entries to disk.
  - Validate header sizes, table counts, block indexes, sector tables, offsets, and lengths before allocating or slicing.
  - Make async decompression verify the exact decompressed length, matching the synchronous path.
  - Add tests for truncated input, invalid tables and sectors, oversized declarations, multi-sector files, Zlib, BZip2, and extraction traversal.

- [x] Fix `s2cli` release-visible behavior, or explicitly exclude it from the library v1.0 scope.
  - Make `--versions` enumerate the generated JSON manifest instead of removed `.py` resources.
  - Allow `--versions` without requiring a replay path.
  - Make `--all` include the replay header.
  - Add CLI behavior tests and build/package the CLI in CI.

## Public API freeze

- [ ] Preserve tracker events in their encoded stream order.
  - Add an ordered `BaseTrackerEvents` collection comparable to game and message events.
  - Keep the existing categorized tracker collections as compatibility views.
  - Retain `UnknownTrackerEvent` instances rather than silently discarding them.

- [ ] Define the JSON contract.
  - Decide whether the complete `Sc2Replay` graph supports serialization only or full round-tripping.
  - If round-tripping is supported, add polymorphic metadata or converters for game and tracker events and test the complete graph.
  - Document the supported JSON contract and its AOT behavior.

- [ ] Resolve pre-1.0 API baggage.
  - Decide whether to remove the obsolete built-in parallel decoding helpers before v1.0.
  - Add the correctly named `ExtractFilesToDisk` API and obsolete or remove `ExtractFilesToDisc`.
  - Review obsolete misspelled attribute aliases and the plural `SCmdUpdateTargetUnitEvents` enum member.
  - Remove or justify the unused `EngineException`.
  - Make `DecodeAsync` non-nullable if a successful decode cannot return `null`.
  - Decide whether `ReplayDecoder` should remain `IDisposable` when it owns no persistent resources.

- [ ] Establish API compatibility enforcement.
  - Generate and review a public API baseline.
  - Add an API compatibility check to CI so accidental breaking changes fail the build.

- [ ] Confirm the target-framework policy for v1.0.
  - Decide whether `net10.0` alone is intentional or whether a broader target such as `net8.0` should also be supported.

## Test and release gates

- [ ] Add at least one real replay for the latest bundled protocol (`97563`).
  - Rename or update the current “latest version” test, whose newest real fixture is `95299`.
  - Continue synthetic coverage across every canonical protocol resource.
  - "C:\Users\pax77\source\repos\s2protocol.NET\s2protocol.NET.tests\replays\test11.SC2Replay"

- [ ] Run the optional stress corpus in CI or in a required pre-release workflow.

- [ ] Raise coverage around public I/O boundaries.
  - Prioritize stream decoding, MPQ read/decompression branches, extraction, cancellation, and malformed input.
  - Record an agreed coverage floor; the audit baseline was 66.09% line and 52.45% branch coverage.

- [ ] Expand the CI release matrix.
  - Build and test the root solution and `s2cli` using a pinned .NET 10 SDK.
  - Pack both distributable artifacts.
  - Publish and execute a Native AOT smoke test against a representative replay.
  - Check NuGet dependencies for known vulnerabilities.
  - Validate package contents and metadata.

- [ ] Add release automation and consistency checks.
  - Keep assembly, file, package, CLI, changelog, and Git tag versions synchronized.
  - Produce symbols/source-link metadata if desired for NuGet debugging.

## Performance and allocation work

All optimizations must be benchmarked against the existing full, tracker-only, message-only, attribute-only, and parallel decode scenarios. Correctness and bounded memory usage take priority over isolated microbenchmark improvements.

- [x] Decrypt MPQ tables in place.
  - Avoid the second full table buffer and the per-word `BitConverter.GetBytes` allocation.
  - Prefer span-based `BinaryPrimitives` reads and writes.

- [x] Replace linear MPQ hash-table scans with the format's hash/probing lookup algorithm.

- [x] Pool transient compressed buffers where ownership and lifetime are unambiguous.
  - Do not pool buffers returned as `ReadOnlyMemory<byte>` unless ownership is represented safely.

- [ ] Prevent duplicate protocol resource initialization during concurrent first use.
  - Ensure only one thread deserializes and builds decoder metadata for each canonical protocol payload.

- [ ] Review collection sizing and compatibility views for avoidable copies and repeated enumeration.

- [ ] Define performance regression budgets for elapsed time and allocated bytes, then enforce or report them in the pre-release workflow.

There are currently no database interactions in this repository, so no database-specific optimization work is required.

## v1.0 completion criteria

- [ ] All release blockers above are closed or explicitly documented as intentionally out of scope.
- [ ] Release, CLI, package, Native AOT, security, and stress checks pass from a clean checkout.
- [ ] The latest supported real replay and the historical replay corpus decode successfully.
- [ ] Public API and JSON contracts are documented and protected by compatibility tests.
- [ ] Performance is no worse than the accepted v0.9.8 baseline beyond agreed variance.
- [ ] README limitations and examples match the shipped behavior.
