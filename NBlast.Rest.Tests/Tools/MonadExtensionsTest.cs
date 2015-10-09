﻿using System;
using FluentAssertions;
using NBlast.Rest.Tools;
using Xunit;

namespace NBlast.Rest.Tests.Tools
{
    public class MonadExtensionsTest
    {
        [Fact(DisplayName = "It should get @true branch when condition returns true")]
        public void Check_If_when_true()
        {
            // given
            var @true = 12345;
            var @false = 54321;

            // when
            var actual = new object().ToMonad()
                .If(_ => true,
                    follow: x => @true,
                    otherwise: x => @false
                ).Value;

            // then
            actual.Should().Be(@true);
        }

        [Fact(DisplayName = "It should get @false branch when condition returns true")]
        public void Check_If_when_false()
        {
            // given
            var @true = 12345;
            var @false = 54321;

            // when
            var actual = new object().ToMonad()
                .If(_ => false,
                    follow: x => @true,
                    otherwise: x => @false
                ).Value;

            // then
            actual.Should().Be(@false);
        }

        [Fact(DisplayName = "It should get an empty monad because of emptiness on previous action")]
        public void Check_If_when_nothing_before()
        {
            // given
            var @true = 12345;
            var @false = 54321;

            // when
            var actual = new object().ToMonad()
                .If(_ => false)
                .If(_ => false,
                    follow: x => @true,
                    otherwise: x => @false
                );

            // then
            actual.HasValue.Should().BeFalse();
        }

        [Fact(DisplayName = "It should get a monad in second condition from previous action")]
        public void Check_If_when_somethings_before()
        {
            // given
            var @true = 12345;
            var @false = 54321;

            // when
            var actual = new object().ToMonad()
                .If(_ => true)
                .If(_ => false,
                    follow: x => @true,
                    otherwise: x => @false
                );

            // then
            actual.HasValue.Should().BeTrue();
            actual.Value.Should().Be(@false);
        }

        [Fact(DisplayName = "It should get an empty monad when one of operands is nothing")]
        public void Check_SelectMany_with_nothing()
        {
            // given
            // when
            var result = from x in 1
                         from y in new int?()
                         select x + y;
            // then
            result.HasValue.Should().BeFalse();
        }

        [Fact(DisplayName = "It should get a calculation result when all operands are the values")]
        public void Check_SelectMany_with_values()
        {
            // given
            // when
            var result = from x in 1
                         from y in 2
                         select x + y;
            // then
            result.HasValue.Should().BeTrue();
            result.Value.Should().Be(3);
        }
    }
}