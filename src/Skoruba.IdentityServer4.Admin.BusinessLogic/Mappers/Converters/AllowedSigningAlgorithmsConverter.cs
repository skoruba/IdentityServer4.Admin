// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

// Modified by Jan Škoruba - original file: https://github.com/IdentityServer/IdentityServer4/blob/main/src/EntityFramework.Storage/src/Mappers/AllowedSigningAlgorithmsConverter.cs

using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers.Converters
{
    public class AllowedSigningAlgorithmsConverter :
        IValueConverter<List<string>, string>,
        IValueConverter<string, List<string>>
    {
        public static AllowedSigningAlgorithmsConverter Converter = new AllowedSigningAlgorithmsConverter();

        public string Convert(List<string> sourceMember, ResolutionContext context)
        {
            if (sourceMember == null || !sourceMember.Any())
            {
                return null;
            }
            return sourceMember.Aggregate((x, y) => $"{x},{y}");
        }

        public List<string> Convert(string sourceMember, ResolutionContext context)
        {
            var list = new List<string>();
            if (!String.IsNullOrWhiteSpace(sourceMember))
            {
                sourceMember = sourceMember.Trim();
                foreach (var item in sourceMember.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Distinct())
                {
                    list.Add(item);
                }
            }
            return list;
        }
    }
}