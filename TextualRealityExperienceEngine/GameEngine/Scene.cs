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
using System.Linq;
using TextualRealityExperienceEngine.GameEngine.Interfaces;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace TextualRealityExperienceEngine.GameEngine 
{
    /// <summary>
    /// Dialogue nodes hold the text for a node in a dialogue tree
    /// </summary>
    public class DialogueNode
    {
        public int Id { get; set; }
        public string Speaker { get; set; }
        public string Text { get; set; }
        public List<Choice> Choices { get; set; }

        /// <summary>
        /// Get choice by id.
        /// </summary>
        /// <param name="choiceId"></param>
        /// <exception cref="ArgumentException">If choice ID is not greater than zero</exception>
        /// <returns>Choice object</returns>
        public Choice GetChoice(int choiceId)
        {
            if (choiceId < 1)
            {
                throw new ArgumentException("Choice id must be greater than zero.", nameof(choiceId));
            }

            return Choices.FirstOrDefault(choice => choice.Id == choiceId);
        }
    }

    /// <summary>
    /// Choice nodes hold the text for a choice in a dialogue tree
    /// </summary>
    public class Choice
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int Next { get; set; }
    }

    /// <summary>
    /// A scene consists of dialogue with choices that the player can make.
    /// </summary>
    public class Scene : IScene
    {
        /// <summary>
        /// Dialogue nodes that the scene holds.
        /// </summary>
        public Dictionary<int, DialogueNode> DialogueNodes { get; set; }

        /// <summary>
        /// Current dialogue node that the player is at.
        /// </summary>
        public DialogueNode CurrentDialogueNode { get; set; }

        /// <summary>
        /// Whether the scene is currently playing
        /// </summary>
        public bool IsPlaying { get; set; }

        /// <summary>
        /// Default Constructor to setup the initial scene state.
        /// </summary>
        public Scene()
        {
            DialogueNodes = new Dictionary<int, DialogueNode>();
            CurrentDialogueNode = null;
            IsPlaying = false;
        }

        /// <summary>
        /// Constructor to setup the initial scene state.
        /// </summary>
        /// <param name="name">Name of the room.</param>
        /// <exception cref="ArgumentNullException">If the name or description are null or empty then throw
        /// an ArgumentNullException.</exception>
        public Scene(string dialogueFile)
        {
            DialogueNodes = new Dictionary<int, DialogueNode>();
            AddDialogueNodesFromFile(dialogueFile);
            IsPlaying = false;
        }

        /// <summary>
        /// Adds dialogue items to the dialogue management system from a YAML file.
        /// </summary>
        /// <param name="identifier">The identifier used to store text and choices in the dialogue system.
        /// This identifier needs to be unique.</param>
        /// <param name="filePath">Path to the YAML file containing dialogue node(s).</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FileNotFoundException">If the specified file cannot be found, a FileNotFoundException
        /// is thrown.</exception>
        public void AddDialogueNodesFromFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The file " + filePath + " does not exist.");
            }

            string yamlContent = File.ReadAllText(filePath);
            var deserializer = new DeserializerBuilder()
                                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                                .Build();
            var dialogueNodes = deserializer.Deserialize<List<DialogueNode>>(yamlContent);

            foreach (var node in dialogueNodes)
            {
                DialogueNodes.Add(node.Id, node);

                // check choices Ids are unique (if present)
                if (node.Choices == null) { continue; }
                List<int> choiceIds = new List<int>();

                foreach (var choice in node.Choices)
                {
                    choiceIds.Add(choice.Id);
                }
                if (choiceIds.Count != choiceIds.Distinct().Count())
                {
                    throw new ArgumentException(
                        "The dialogue node " + node.Id + " contains duplicate choice Ids."
                    );
                }
            }

            InitialiseScene();
        }

        /// <summary>>
        /// Initialise the scene at the first dialogue node.
        /// </summary>
        /// <exception cref="ArgumentException">If there are no dialogue nodes.</exception>
        public void InitialiseScene()
        {
            if (DialogueNodes.Count == 0)
            {
                throw new ArgumentException(nameof(DialogueNodes), "No dialogue nodes assigned to the scene.");
            }
            int minKey = DialogueNodes.Keys.Min();
            CurrentDialogueNode = DialogueNodes[minKey];
        }

        /// <summary>
        /// Get the current dialogue node's text.
        /// </summary>
        /// <returns>Current dialogue node's text</returns>
        public string GetTextForCurrentDialogueNode()
        {
            if (CurrentDialogueNode == null)
            {
                return String.Empty;
            }

            if (CurrentDialogueNode.Speaker == null)
            {
                return CurrentDialogueNode.Text.Trim();
            }
            else
            {
                return CurrentDialogueNode.Speaker + ": \"" + CurrentDialogueNode.Text.Trim() + "\"";
            }
        }

        /// <summary>
        /// Get the current dialogue node's choices.
        /// </summary>
        /// <returns></returns>
        // TODO: in the future, this will take the game object and
        // check whether each choice is valid given some criteria
        // specified inthe dialogue file, and the game state.
        public List<(int displayId, Choice choice)> GetValidChoicesForCurrentDialogueNode()
        {
            if (CurrentDialogueNode == null)
            {
                return new List<(int, Choice)>();
            }
            var choices = CurrentDialogueNode.Choices;
            List<(int displayId, Choice choice)> validChoices = new();
            if (choices == null)
            {
                return validChoices;
            }

            int choiceCounter = 1;
            foreach (var choice in choices)
            {
                validChoices.Add((choiceCounter, choice));
                choiceCounter++;
            }
            return validChoices;
        }

        /// <summary>
        /// Set the next dialoge node based on the player selected choice.
        /// </summary>
        /// <returns>True if setting the next nde was successful, otherwise false.</returns>
        public bool ProcessDialogueChoice(List<(int displayId, Choice choice)> validChoices, string dialogueReply)
        {
            if (CurrentDialogueNode == null)
            {
                return false;
            }

            Choice choice = null;
            dialogueReply = dialogueReply.Trim();
            if (dialogueReply == String.Empty && validChoices.Count == 1)
            {
                choice = validChoices.First().choice;
            } 
            else
            {
                int.TryParse(dialogueReply, out int playerSelectId);
                if (validChoices.Exists(tuple => tuple.displayId == playerSelectId))
                {
                    choice = validChoices.Find(tuple => tuple.displayId == playerSelectId).choice;
                }
            }

            // handle case where there are no valid choices
            if (validChoices.Count == 0)
            {
                if (DialogueNodes.ContainsKey(CurrentDialogueNode.Id + 1))
                {
                    CurrentDialogueNode = DialogueNodes[CurrentDialogueNode.Id + 1];
                }
                else
                {
                    IsPlaying = false;
                }
                return true;
            }

            switch (choice)
            {
                case null:
                    return false;
                case { Next: 0 }:
                    InitialiseScene(); // go back to the start of the scene
                    IsPlaying = false;
                    return true;
                default:
                    CurrentDialogueNode = DialogueNodes[choice.Next];
                    return true;
            }
        }
    }
}
