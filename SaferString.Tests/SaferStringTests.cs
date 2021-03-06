﻿using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SaferString.Tests
{
    [TestClass]
    public class SaferStringTests
    {
        [TestMethod]
        public void ToSecureString__ReturnsSecureString()
        {
            const string str = "Foobar";
            var secStr = str.ToSecureString();

            Assert.IsNotNull(secStr);
        }

        [TestMethod]
        public void ToSecureString__ClearsOriginalStringAfterConvert()
        {
            // ReSharper disable once ConvertToConstant.Local
            var str = "Llama";
            Assert.IsFalse(String.IsNullOrEmpty(str));
            str.ToSecureString();
            Assert.IsTrue(String.IsNullOrEmpty(str));
        }

        [TestMethod]
        public void ToString__ReturnsUnencryptedString()
        {
            var chr = new List<char>{'L', 'l', 'a', 'm', 'a'};
            var secStr = new SecureString();
            chr.ForEach(secStr.AppendChar);

            var str = secStr.ToUnsecureString();
            Assert.AreEqual(String.Join("", chr), str);
        }

        [TestMethod]
        public void EraseString__ErasesTheGivenString()
        {
            // ReSharper disable once ConvertToConstant.Local
            var str = "We're going to Candy Mountain.";
            Assert.IsFalse(String.IsNullOrWhiteSpace(str));
            str.Zero();
            Assert.IsTrue(String.IsNullOrWhiteSpace(str));
        }

        [TestMethod]
        public void EraseString__CanEraseConst()
        {
            const String str = "Tho shall not pass!";
            Assert.IsFalse(String.IsNullOrWhiteSpace(str));
            str.Zero();
            Assert.IsTrue(String.IsNullOrWhiteSpace(str));
        }

        [TestMethod]
        public void Lambda__AllowsRunningArbitraryCodeWithReturn()
        {
            var chr = new List<char> { 'L', 'l', 'a', 'm', 'a' };
            var secStr = new SecureString();
            chr.ForEach(secStr.AppendChar);
            const int expectedValue = 0xBADBEF;

            Func<String, int> lambda = s =>
            {
                if (s == "Llama")
                    return expectedValue;
                return 0x0;
            };

            var results = secStr.Lambda<int>(lambda);

            Assert.AreEqual(expectedValue, results);
        }

        [TestMethod]
        public void Lambda__AllowsRunningArbitraryCodeAndParamsWithReturn()
        {
            var chr = new List<char> { 'L', 'l', 'a', 'm', 'a' };
            var secStr = new SecureString();
            chr.ForEach(secStr.AppendChar);
            const int expectedValue = 0xBADBEF;

            Func<String, int, int> lambda = (s, i) =>
            {
                if (s == "Llama")
                    return i;
                return 0x0;
            };

            var results = secStr.Lambda<int>(lambda, expectedValue);

            Assert.AreEqual(expectedValue, results);
        }

        [TestMethod]
        public void Lambda__AllowsRunningArbitraryCodeWithoutReturn()
        {
            var cache = new List<String>();
            // ReSharper disable InconsistentNaming
            // ReSharper disable ConvertToConstant.Local
            var expectedString_1 = "Secret";
            var expectedString_2 = "TopSecret";
            // ReSharper restore InconsistentNaming
            // ReSharper restore ConvertToConstant.Local

            Action<String> lambda = delegate(String s)
            {
                var sb = new StringBuilder(2);
                sb.Append("Top");
                sb.Append(s);
                cache.Add($"{s}");
                cache.Add(sb.ToString());
            };

            var chr = new List<char> { 'S', 'e', 'c', 'r', 'e', 't' };
            var secStr = new SecureString();
            chr.ForEach(secStr.AppendChar);

            secStr.Lambda(lambda);

            CollectionAssert.Contains(cache, expectedString_1);
            CollectionAssert.Contains(cache, expectedString_2);
        }

        [TestMethod]
        public void Lambda__AllowsRunningArbitraryCodeAndParamsWithoutReturn()
        {
            var cache = new List<String>();
            // ReSharper disable InconsistentNaming
            // ReSharper disable ConvertToConstant.Local
            var expectedString_1 = "Secret";
            var expectedString_2 = "TopSecret";
            // ReSharper restore InconsistentNaming
            // ReSharper restore ConvertToConstant.Local

            Action<String, String> lambda = (s, prefix) =>
            {
                var sb = new StringBuilder(2);
                sb.Append(prefix);
                sb.Append(s);
                cache.Add($"{s}");
                cache.Add(sb.ToString());
            };

            var chr = new List<char> { 'S', 'e', 'c', 'r', 'e', 't' };
            var secStr = new SecureString();
            chr.ForEach(secStr.AppendChar);

            secStr.Lambda(lambda, "Top");

            CollectionAssert.Contains(cache, expectedString_1);
            CollectionAssert.Contains(cache, expectedString_2);
        }
    }
}