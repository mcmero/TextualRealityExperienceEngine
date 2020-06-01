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
using TextualRealityExperienceEngine.GameEngine;
using TextualRealityExperienceEngine.GameEngine.Interfaces;
using TextualRealityExperienceEngine.GameEngine.TextProcessing.Interfaces;

namespace TextualRealityExperienceEngine.Tests.SimpleGame.Library.UpStairs
{
    public class SmallBedroom : Room
    {
        public SmallBedroom(IGame game) : base(game)
        {
            game.ContentManagement.AddContentItem("SmallBedroomName", "SmallBedroom");
            game.ContentManagement.AddContentItem("SmallBedroomDescription", "You are standing in a childs bedroom.");

            Name = game.ContentManagement.RetrieveContentItem("SmallBedroomName");
            Description = game.ContentManagement.RetrieveContentItem("SmallBedroomDescription");
        }

        public override string ProcessCommand(ICommand command)
        {
            string reply;

            if (command.ProfanityDetected)
            {
                return Game.ContentManagement.RetrieveContentItem("NoNeedToBeRude");
            }                     

            reply = base.ProcessCommand(command);

            return reply;
        }
    }
}