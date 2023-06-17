/*
MIT License

Copyright (c) 2019 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextualRealityExperienceEngine.GameEngine;
using System;

namespace TextualRealityExperienceEngine.Tests.Unit.GameEngine
{
    [TestClass]
    public class CharacterTests
    {
        [TestMethod]
        public void DefaultConstructorCreatesEmptyNameAndEmptyDescription()
        {
            var Character = new Character();
            Assert.AreEqual(string.Empty, Character.Name);
            Assert.AreEqual(string.Empty, Character.Description);
        }

        [TestMethod]
        public void DefaultConstructorSetGenderIdentityToOther()
        {
            var Character = new Character();
            Assert.AreEqual(GenderIdentityEnum.Other, Character.GenderIdentity);
        }

        [TestMethod]
        public void ConstructorCreatesSetsName()
        {
            var Character = new Character("Geoff");
            Assert.AreEqual("Geoff", Character.Name);
        }

        [TestMethod]
        public void ConstructorSetsGenderToOtherByDefault()
        {
            var Character = new Character("Geoff");
            Assert.AreEqual(GenderIdentityEnum.Other, Character.GenderIdentity);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Name of Character can not be null.")]
        public void ConstructorCreatesThrowsArgumentNullExceptionIfNameIsNull()
        {
            var Character = new Character(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Name of Character can not be null.")]
        public void ConstructorCreatesThrowsArgumentNullExceptionIfNameIsEmptyString()
        {
            var Character = new Character(string.Empty);
        }

        [TestMethod]
        public void ConstructorSetsGenderToMaleAndNameToArther()
        {
            var Character = new Character("Arther", "Knight", GenderIdentityEnum.Male);
            Assert.AreEqual("Arther", Character.Name);
            Assert.AreEqual("Knight", Character.Description);
            Assert.AreEqual(GenderIdentityEnum.Male, Character.GenderIdentity);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Name of Character can not be null.")]
        public void ConstructorCreatesThrowsArgumentNullExceptionIfNameIsNullWithGenderSetToOther()
        {
            var Character = new Character(null, String.Empty, GenderIdentityEnum.Other);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException), "Name of Character can not be null.")]
        public void ConstructorCreatesThrowsArgumentNullExceptionIfNameIsEmptyStringWithGenderSetToOther()
        {
            var Character = new Character(string.Empty, String.Empty, GenderIdentityEnum.Other);
        }
    }
}
