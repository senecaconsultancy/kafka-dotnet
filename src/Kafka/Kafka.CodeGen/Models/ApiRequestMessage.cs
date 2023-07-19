﻿using Kafka.Common.Model;
using System.Collections.Immutable;
using VersionRange = Kafka.Common.Model.VersionRange;

namespace Kafka.CodeGen.Models
{
    public sealed record ApiRequestMessage(
        ApiKey ApiKey,
        string[] Listeners,
        string Name,
        VersionRange ValidVersions,
        VersionRange FlexibleVersions,
        ImmutableArray<Field> Fields,
        IImmutableDictionary<string, StructDefinition> Structs
    ) :
        ApiMessage(
            Name,
            ApiKey,
            ValidVersions,
            FlexibleVersions,
            Fields,
            Structs
        )
    ;
}
