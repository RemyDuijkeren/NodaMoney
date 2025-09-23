# Feature Proposal: AOT & Trimming Considerations

Status: proposal/spec
Source: docs/feature-cldr-formatting-and-parsing.md

## Objectives
Ensure all formatting/parsing providers—especially the CLDR subset implementation—are compatible with trimming and native AOT across the supported TFMs.

## Guidelines
- Avoid reflection-based access paths; no runtime type discovery for data tables.
- Prefer source-generated code tables or fixed-layout binary resources with a tiny loader using spans.
- Use System.Collections.Frozen on net8+/net9+/net10 with compile-time directives and safe fallbacks.
- Avoid culture lookup via reflection; normalize culture names and use static parent maps.
- Keep public surface small to minimize trimming roots.

## Possible Implementations
- Codegen data (preferred): readonly arrays + FrozenDictionary indexes; no IO required.
- Binary data: ReadOnlyMemory<byte> with BinaryPrimitives; only allocate strings when needed.
- Multi-TFM conditionals for FrozenDictionary; ImmutableDictionary on older TFMs.

## Diagnostics
- Expose CldrInfo.Version and ProviderInfo.Name for debugging; ensure they are const/readonly.

## Acceptance Criteria
- Builds with PublishTrimmed=true and PublishAot=true in test projects.
- No linker warnings in default configuration.
- Deterministic behavior verified in AOT runs on Windows/Linux.
