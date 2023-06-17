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
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TextualRealityExperienceEngine.GameEngine;

namespace TextualRealityExperienceEngine.Tests.Unit.GameEngine
{
    [TestClass]
    public class SceneTests
    {
        [TestMethod]
        public void DialogueNodesAreEmptyWithDefaultConstructor()
        {
            var scene = new Scene();
            Assert.AreEqual(0, scene.DialogueNodes.Count);
        }

        [TestMethod]
        public void CurrentDialogueNodeIsNullWithDefaultConstructor()
        {
            var scene = new Scene();
            Assert.IsNull(scene.CurrentDialogueNode);
        }

        [TestMethod]
        public void CreateDialogueNodesFromFileWithGameConstructor()
        {
            var scene = new Scene(Path.Combine("TestFiles", "TestDialogue.yaml"));
            Assert.AreEqual(1, scene.DialogueNodes.Count);
        }

        [TestMethod]
        public void AddDialogueNodesFromFileCountNodes()
        {
            var scene = new Scene();
            scene.AddDialogueNodesFromFile(Path.Combine("TestFiles", "TestDialogue.yaml"));
            Assert.AreEqual(1, scene.DialogueNodes.Count);
            Assert.AreEqual(1, scene.CurrentDialogueNode.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddDialogueNodesFromFileWithDuplicateNodeIds()
        {
            var scene = new Scene();
            scene.AddDialogueNodesFromFile(Path.Combine("TestFiles", "TestDialogueDuplicateNodeId.yaml"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddDialogueNodesFromFileWithDuplicateChoiceIds()
        {
            var scene = new Scene();
            scene.AddDialogueNodesFromFile(Path.Combine("TestFiles", "TestDialogueDuplicateChoiceId.yaml"));
        }

        [TestMethod]
        public void GetPropertiesForCurrentDialogueNode()
        {
            var scene = new Scene();
            scene.AddDialogueNodesFromFile(Path.Combine("TestFiles", "TestDialogue.yaml"));
            Assert.AreEqual(1, scene.CurrentDialogueNode.Id);
            Assert.AreEqual("A man", scene.CurrentDialogueNode.Speaker);
            Assert.AreEqual("A man: \"Hello, traveller.\"", scene.GetTextForCurrentDialogueNode());
        }

        [TestMethod]
        public void GetTextForCurrentDialogueNodeTestNarrator()
        {
            var scene = new Scene();
            scene.AddDialogueNodesFromFile(Path.Combine("TestFiles", "TestDialogueWithNarration.yaml"));
            scene.CurrentDialogueNode = scene.DialogueNodes[2];
            Assert.AreEqual("The man disappears into a mist.", scene.GetTextForCurrentDialogueNode());
        }

        [TestMethod]
        public void AdvanceDialogueNodeWithNoChoices()
        {
            var scene = new Scene();
            scene.AddDialogueNodesFromFile(Path.Combine("TestFiles", "TestDialogueNoChoices.yaml"));
            var validChoices = scene.GetValidChoicesForCurrentDialogueNode();
            scene.ProcessDialogueChoice(validChoices, "");
            Assert.AreEqual(2, scene.CurrentDialogueNode.Id);
        }

        [TestMethod]
        public void GetValidChoicesForCurrentDialogueNode()
        {
            var scene = new Scene();
            scene.AddDialogueNodesFromFile(Path.Combine("TestFiles", "TestDialogue.yaml"));
            var choices = scene.GetValidChoicesForCurrentDialogueNode();
            Assert.AreEqual(2, choices.Count);
            Assert.AreEqual(1, choices[0].displayId);
            Assert.AreEqual(1, choices[0].choice.Id);
            Assert.AreEqual("Who are you?", choices[0].choice.Text);
            Assert.AreEqual(0, choices[0].choice.Next);
            Assert.AreEqual(2, choices[1].displayId);
            Assert.AreEqual(2, choices[1].choice.Id);
            Assert.AreEqual("What is your name?", choices[1].choice.Text);
            Assert.AreEqual(0, choices[1].choice.Next);
        }

        [TestMethod]
        public void GetChoiceCheckText()
        {
            var scene = new Scene();
            scene.AddDialogueNodesFromFile(Path.Combine("TestFiles", "TestDialogue.yaml"));
            Choice choice1 = scene.DialogueNodes[1].GetChoice(1);
            Choice choice2 = scene.DialogueNodes[1].GetChoice(2);
            Assert.AreEqual("Who are you?", choice1.Text);
            Assert.AreEqual("What is your name?", choice2.Text);
        }

        [TestMethod]
        public void GetChoiceNullIfChoiceDoesNotExist()
        {
            var scene = new Scene();
            scene.AddDialogueNodesFromFile(Path.Combine("TestFiles", "TestDialogue.yaml"));
            Choice choice = scene.DialogueNodes[1].GetChoice(3);
            Assert.IsNull(choice);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetChoiceThrowsArgumentExceptionIfIdLessThanZero()
        {
            var scene = new Scene();
            scene.AddDialogueNodesFromFile(Path.Combine("TestFiles", "TestDialogue.yaml"));
            scene.DialogueNodes[1].GetChoice(-1);
        }

        [TestMethod]
        public void InitialiseSceneWithLowestId()
        {
            var scene = new Scene();
            scene.AddDialogueNodesFromFile(Path.Combine("TestFiles", "TestDialogue.yaml"));
            Assert.AreEqual(1, scene.CurrentDialogueNode.Id);
        }

        [TestMethod]
        public void InitialiseSceneWithLowestIdAndText()
        {
            var scene = new Scene();
            scene.AddDialogueNodesFromFile(Path.Combine("TestFiles", "TestDialogue.yaml"));
            Assert.AreEqual("Hello, traveller.", scene.CurrentDialogueNode.Text);
        }

        [TestMethod]
        public void ProcessDialogueChoiceProceedDefaultWithOneChoice()
        {
            var scene = new Scene();
            scene.AddDialogueNodesFromFile(Path.Combine("TestFiles", "TestDialogueTwoNodes.yaml"));
            scene.IsPlaying = true;
            scene.CurrentDialogueNode = scene.DialogueNodes[2];
            var validChoices = scene.GetValidChoicesForCurrentDialogueNode();
            scene.ProcessDialogueChoice(validChoices, "");
            Assert.AreEqual(1, scene.CurrentDialogueNode.Id);
            Assert.IsFalse(scene.IsPlaying);
        }

        [TestMethod]
        public void ProcessDialogueChoiceProceed()
        {
            var scene = new Scene();
            scene.AddDialogueNodesFromFile(Path.Combine("TestFiles", "TestDialogueTwoNodes.yaml"));
            scene.IsPlaying = true;
            var validChoices = scene.GetValidChoicesForCurrentDialogueNode();
            Assert.AreEqual(1, scene.CurrentDialogueNode.Id);
            scene.ProcessDialogueChoice(validChoices, "1");
            Assert.AreEqual(2, scene.CurrentDialogueNode.Id);
            Assert.IsTrue(scene.IsPlaying);
        }

        [TestMethod]
        public void ProcessDialogueChoiceEndScene()
        {
            var scene = new Scene();
            scene.AddDialogueNodesFromFile(Path.Combine("TestFiles", "TestDialogueTwoNodes.yaml"));
            scene.IsPlaying = true;
            var validChoices = scene.GetValidChoicesForCurrentDialogueNode();
            Assert.AreEqual(1, scene.CurrentDialogueNode.Id);
            scene.ProcessDialogueChoice(validChoices, "2");
            Assert.AreEqual(1, scene.CurrentDialogueNode.Id);
            Assert.IsFalse(scene.IsPlaying);
        }
    }

}
