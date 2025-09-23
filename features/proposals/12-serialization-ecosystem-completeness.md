# Serialization Ecosystem Completeness

Why it matters
- Interop needs across serializers (System.Text.Json, Newtonsoft.Json, BSON/Mongo) and AOT readiness are common. Stable, lossless wire formats reduce ambiguity.

Proposal
- Provide parity converters (STJ, Newtonsoft, BSON), source-gen support, and a stable canonical JSON schema that is lossless and optionally includes context metadata.

Possible implementation
- Converters:
  - System.Text.Json: ensure converters for Money and CurrencyInfo; add source-generated JsonSerializerContext examples.
  - Newtonsoft.Json: parity converters mirroring STJ behavior.
  - Mongo/BSON: Money serializer with string amount to avoid precision loss.
- Canonical schema option:
  - { "currencyCode": "USD", "amount": "1234.56", "contextName": "default"? }
  - Allow opt-in via options for string amounts (lossless) vs numeric for convenience.
- AOT-friendliness:
  - Provide sample JsonSerializerContext and linker descriptors if needed.
- Tests:
  - Round-trip tests across serializers and TFMs; ensure culture-invariant behavior.

Risks / considerations
- Backward compatibility with existing serialized forms; consider versioned schema.
- Precision loss if numeric amounts are used; recommend string for canonical wire format.

Open questions
- Should contextName be emitted/consumed by default or only when requested?
- Do we include currency minor units or metadata for display scale?
