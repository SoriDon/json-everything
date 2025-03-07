---
layout: page
title: JsonPatch.Net
icon: fas fa-tag
order: "09.08"
---
# [3.0.0](https://github.com/gregsdennis/json-everything/pull/619) {#release-patch-3.0.0}

Updated for multi-framework support with .Net Standard 2.0 and .Net 8 with Native AOT support, including updating _System.Text.Json_ from v6 to v8.

Most of the changes to support Native AOT involve either updating internal implementation or creating overloads that do.  Whatever could not be updated was marked as requiring reflection, which will generate warnings when building Native AOT applications.

## Breaking changes

Dropping support for .Net Standard 3.1 - May still be used, but exact behavior cannot be guaranteed. 

## Additions

`JsonPatch.TypeInfoResolver` to expose all of the type resolvers contained in the library.  Can be used to create a combined `SerializationOptions` by using a `Json.More.TypeResolverOptionsManager` in your `JsonSerializerContext`.

## Other chanages

`PatchExtensions.Apply()` and `.Create()`, including all overloads, have been marked as AOT-incompatible since they use unsupported reflection.

# [2.1.0](https://github.com/gregsdennis/json-everything/pull/472) {#release-patch-2.1.0}

[#471](https://github.com/gregsdennis/json-everything/issues/397) - Make patch json converter public to support .Net source generation.  Thanks to [@pwelter34](https://github.com/pwelter34) for highlighting this use case.

# [2.0.6](https://github.com/gregsdennis/json-everything/pull/400) {#release-patch-2.0.6}

[#397](https://github.com/gregsdennis/json-everything/issues/397) - Fixed an issue where `replace` needs to check that the target location exists before proceeding with the `add` portion of its operation.

# [2.0.5](https://github.com/gregsdennis/json-everything/pull/394) {#release-patch-2.0.5}

[#393](https://github.com/gregsdennis/json-everything/issues/393) - Fixed an `InvalidOperationException` from some of the operations.

# [2.0.4](https://github.com/gregsdennis/json-everything/pull/323) {#release-patch-2.0.4}

[#322](https://github.com/gregsdennis/json-everything/pull/322) - [@z4kn4fein](https://github.com/z4kn4fein) discovered and fixed an issue in the `move` operation logic.

# [2.0.3](https://github.com/gregsdennis/json-everything/pull/317) {#release-patch-2.0.3}

[#315](https://github.com/gregsdennis/json-everything/pull/315) - [@z4kn4fein](https://github.com/z4kn4fein) noticed that the serializer options weren't actually being passed into the `.Apply()` call.

# 2.0.2 (no PR) {#release-patch-2.0.2}

[#291](https://github.com/gregsdennis/json-everything/pull/291) - Improved patch generation for arrays.

# 2.0.1 (no PR) {#release-patch-2.0.1}

[#288](https://github.com/gregsdennis/json-everything/issues/288) - Just bumping version to pick up the latest Json.More.Net by default.  This package pull Json.More.Net transitively via JsonPointer.Net which wasn't updated with the move to `JsonNode`.

# [2.0.0](https://github.com/gregsdennis/json-everything/pull/280) {#release-patch-2.0.0}

[#243](https://github.com/gregsdennis/json-everything/pull/243) - Updated System.Text.Json to version 6.

Updated all functionality to use `JsonNode` instead of `JsonElement`.

## Breaking Changes {#release-patch-2.0.0-breaks}

_`JsonElement` -> `JsonNode` type exchange changes not listed._

- `JsonPatch.Apply()` now takes `JsonNode`
- `.CreatePatch(JsonDocument, JsonDocument)` removed
- `.CreatePatch(JsonElement, JsonElement)` replaced with `.CreatePatch(JsonNode?, JsonNode?)`
- `PatchOperation` converted to a class
- `PatchOperation` static methods which take `JsonElementProxy` removed as `JsonNode` defines implicit casts for the supported types
- `PatchResult.Result` update to `JsonNode?`

## Additional Changes {#release-patch-1.7.0-additional}

- `.Apply<T>()` extension method now takes optional serializer options

# [1.1.2](https://github.com/gregsdennis/json-everything/pull/196) {#release-patch-1.1.2}

[#192](https://github.com/gregsdennis/json-everything/pull/192) - [@LordXaosa](https://github.com/LordXaosa) found some issues with patch generation.

# [1.1.1](https://github.com/gregsdennis/json-everything/pull/179) {#release-patch-1.1.1}

Updated JsonPointer.Net to v2.0.0.  Please see [release notes](./json-pointer.md) for that library as it contains breaking changes.

# [1.1.0](https://github.com/gregsdennis/json-everything/pull/163) {#release-patch-1.1.0}

[#160](https://github.com/gregsdennis/json-everything/pull/160) - Added JSON Patch creation via comparison of objects or JSON data.  Credit for implementation to [@LordXaosa](https://github.com/LordXaosa).

Added `JsonElementProxy` overloads for `PatchOperation.Add()`, `PatchOperation.Replace()`, and `PatchOperation.Test()`.

# [1.0.6](https://github.com/gregsdennis/json-everything/pull/147) {#release-patch-1.0.6}

[#132](https://github.com/gregsdennis/json-everything/pull/132) (Fixed on [#133](https://github.com/gregsdennis/json-everything/pull/133)) - Fixed some memory management issues around `JsonDocument` and `JsonElement`.  Thanks to [@ddunkin](https://github.com/ddunkin) for finding and fixing these.

[#146](https://github.com/gregsdennis/json-everything/issues/146) - Fixed an issue during operation construction that appeared when attempting to use JSON Patch in an MVC controller.

# [1.0.5](https://github.com/gregsdennis/json-everything/pull/75) {#release-patch-1.0.5}

Added support for nullable reference types.

# [1.0.4](https://github.com/gregsdennis/json-everything/pull/61) {#release-patch-1.0.4}

Signed the DLL for strong name compatibility.

# [1.0.3](https://github.com/gregsdennis/json-everything/commit/4b6c5900f4bfb45119a3dc5c3ce60b7d7a2e8c9e) {#release-patch-1.0.3}

Bump for publish.  No functional change.

# [1.0.2](https://github.com/gregsdennis/json-everything/pull/45) {#release-patch-1.0.2}

Added debug symbols to package.  No functional change.

# [1.0.1](https://github.com/gregsdennis/json-everything/pull/26) {#release-patch-1.0.1}

Implemented patch equality.

# 1.0.0 {#release-patch-1.0.0}

Initial release.
