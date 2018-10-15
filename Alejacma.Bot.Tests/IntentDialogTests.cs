using Alejacma.Bot.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Adapters;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using System.IO;
using System.Collections.Generic;
using System;

/// <summary>
/// Code based on https://github.com/Microsoft/botbuilder-dotnet/blob/master/tests/Microsoft.Bot.Builder.Dialogs.Tests/WaterfallTests.cs
/// & https://github.com/Microsoft/botbuilder-dotnet/blob/master/tests/Microsoft.Bot.Builder.AI.LUIS.Tests/LuisRecognizerTests.cs
/// </summary>
namespace Alejacma.Bot.Tests
{
    [TestClass]
    public class IntentDialogTests
    {
        [TestMethod]
        public async Task NoWaterfallsAsync()
        {
            var luisRecognizer = GetLuisRecognizer(verbose: true);

            var dialog = new IntentDialog("test", luisRecognizer);

            await GetTestFlow(dialog)
                .Send("hello")
                .StartTestAsync();
        }

        [TestMethod]
        public async Task BeginOnly_OneStepAsync()
        {
            var luisRecognizer = GetLuisRecognizer(verbose: true);

            var dialog = new IntentDialog("test", luisRecognizer)
                .Begin(async (step, cancellationToken) => { await step.Context.SendActivityAsync("begin"); return Dialog.EndOfTurn; });

            await GetTestFlow(dialog)
                .Send("hello")
                .AssertReply("begin")
                .Send("bye")
                .AssertReply("begin")
                .StartTestAsync();
        }

        [TestMethod]
        public async Task BeginOnly_MultipleStepsWithEndOfTurnAsync()
        {
            var luisRecognizer = GetLuisRecognizer(verbose: true);

            var dialog = new IntentDialog("test", luisRecognizer)
                .Begin(new WaterfallStep[]
                {
                    async (step, cancellationToken) => { await step.Context.SendActivityAsync("begin1"); return Dialog.EndOfTurn; },
                    async (step, cancellationToken) => { await step.Context.SendActivityAsync("begin2"); return Dialog.EndOfTurn; },
                });

            await GetTestFlow(dialog)
                .Send("hello")
                .AssertReply("begin1")
                .Send("bye")
                .AssertReply("begin1")
                .StartTestAsync();
        }

        [TestMethod]
        public async Task BeginOnly_MultipleStepsWithNextAsync()
        {
            var luisRecognizer = GetLuisRecognizer(verbose: true);

            var dialog = new IntentDialog("test", luisRecognizer)
                .Begin(new WaterfallStep[]
                {
                    async (step, cancellationToken) => { await step.Context.SendActivityAsync("begin1"); return await step.NextAsync(); },
                    async (step, cancellationToken) => { await step.Context.SendActivityAsync("begin2"); return Dialog.EndOfTurn; },

                });

            await GetTestFlow(dialog)
                .Send("hello")
                .AssertReply("begin1")
                .AssertReply("begin2")
                .Send("bye")
                .AssertReply("begin1")
                .AssertReply("begin2")
                .StartTestAsync();
        }

        [TestMethod]
        public async Task DefaultOnly_OneStepAsync()
        {
            var luisRecognizer = GetLuisRecognizer(verbose: true);

            var dialog = new IntentDialog("test", luisRecognizer)
                .Default(async (step, cancellationToken) => { await step.Context.SendActivityAsync("default"); return Dialog.EndOfTurn; });

            await GetTestFlow(dialog)
                .Send("hello")
                .AssertReply("default")
                .Send("bye")
                .AssertReply("default")
                .StartTestAsync();
        }

        [TestMethod]
        public async Task DefaultOnly_MultipleStepsWithEndOfTurnAsync()
        {
            var luisRecognizer = GetLuisRecognizer(verbose: true);

            var dialog = new IntentDialog("test", luisRecognizer)
                .Begin(new WaterfallStep[]
                {
                    async (step, cancellationToken) => { await step.Context.SendActivityAsync("default1"); return Dialog.EndOfTurn; },
                    async (step, cancellationToken) => { await step.Context.SendActivityAsync("default2"); return Dialog.EndOfTurn; },
                });

            await GetTestFlow(dialog)
                .Send("hello")
                .AssertReply("default1")
                .Send("bye")
                .AssertReply("default1")
                .StartTestAsync();
        }

        [TestMethod]
        public async Task DefaultOnly_MultipleStepsWithNextAsync()
        {
            var luisRecognizer = GetLuisRecognizer(verbose: true);

            var dialog = new IntentDialog("test", luisRecognizer)
                .Begin(new WaterfallStep[]
                {
                    async (step, cancellationToken) => { await step.Context.SendActivityAsync("default1"); return await step.NextAsync(); },
                    async (step, cancellationToken) => { await step.Context.SendActivityAsync("default2"); return Dialog.EndOfTurn; },
                });

            await GetTestFlow(dialog)
                .Send("hello")
                .AssertReply("default1")
                .AssertReply("default2")
                .Send("bye")
                .AssertReply("default1")
                .AssertReply("default2")
                .StartTestAsync();
        }

        [TestMethod]
        public async Task OneIntentHandler_OneStepAsync()
        {
            var luisRecognizer = GetLuisRecognizer(verbose: true);

            var dialog = new IntentDialog("test", luisRecognizer)
                .Begin(async (step, cancellationToken) => { await step.Context.SendActivityAsync("begin"); return Dialog.EndOfTurn; })
                .Matches("Hi", async (step, cancellationToken) => { await step.Context.SendActivityAsync("hi"); return Dialog.EndOfTurn; })
                .Default(async (step, cancellationToken) => { await step.Context.SendActivityAsync("default"); return Dialog.EndOfTurn; });

            await GetTestFlow(dialog)
                .Send("hello")
                .AssertReply("begin")
                .Send("hello")
                .AssertReply("hi")
                .Send("hello")
                .AssertReply("hi")
                .Send("bye")
                .AssertReply("default")
                .StartTestAsync();
        }

        [TestMethod]
        public async Task OneIntentHandler_MultipleStepsWithEndOfTurnAsync()
        {
            var luisRecognizer = GetLuisRecognizer(verbose: true);

            var dialog = new IntentDialog("test", luisRecognizer)
                .Begin(async (step, cancellationToken) => { await step.Context.SendActivityAsync("begin"); return Dialog.EndOfTurn; })
                .Matches("Hi", new WaterfallStep[]
                {
                    async (step, cancellationToken) => { await step.Context.SendActivityAsync("hi1"); return Dialog.EndOfTurn; },
                    async (step, cancellationToken) => { await step.Context.SendActivityAsync("hi2"); return Dialog.EndOfTurn; },
                })
                .Default(async (step, cancellationToken) => { await step.Context.SendActivityAsync("default"); return Dialog.EndOfTurn; });

            await GetTestFlow(dialog)
                .Send("hello")
                .AssertReply("begin")
                .Send("hello")
                .AssertReply("hi1")
                .Send("hello")
                .AssertReply("hi1")
                .Send("bye")
                .AssertReply("default")
                .StartTestAsync();
        }

        [TestMethod]
        public async Task OneIntentHandler_MultipleStepsWithNextAsync()
        {
            var luisRecognizer = GetLuisRecognizer(verbose: true);

            var dialog = new IntentDialog("test", luisRecognizer)
                .Begin(async (step, cancellationToken) => { await step.Context.SendActivityAsync("begin"); return Dialog.EndOfTurn; })
                .Matches("Hi", new WaterfallStep[]
                {
                    async (step, cancellationToken) => { await step.Context.SendActivityAsync("hi1"); return await step.NextAsync(); },
                    async (step, cancellationToken) => { await step.Context.SendActivityAsync("hi2"); return Dialog.EndOfTurn; },
                })
                .Default(async (step, cancellationToken) => { await step.Context.SendActivityAsync("default"); return Dialog.EndOfTurn; });

            await GetTestFlow(dialog)
                .Send("hello")
                .AssertReply("begin")
                .Send("hello")
                .AssertReply("hi1")
                .AssertReply("hi2")
                .Send("hello")
                .AssertReply("hi1")
                .AssertReply("hi2")
                .Send("bye")
                .AssertReply("default")
                .StartTestAsync();
        }

        [TestMethod]
        public async Task OneIntentHandler_MultipleStepsWithUnexpectedNextAsync()
        {
            var luisRecognizer = GetLuisRecognizer(verbose: true);

            var dialog = new IntentDialog("test", luisRecognizer)
                .Begin(async (step, cancellationToken) => { await step.Context.SendActivityAsync("begin"); return Dialog.EndOfTurn; })
                .Matches("Hi", async (step, cancellationToken) => { await step.Context.SendActivityAsync("hi"); return await step.NextAsync(); })
                .Default(async (step, cancellationToken) => { await step.Context.SendActivityAsync("default"); return Dialog.EndOfTurn; });

            try
            {

                await GetTestFlow(dialog)
                    .Send("hello")
                    .AssertReply("begin")
                    .Send("hello")
                    .AssertReply("hi")
                    .StartTestAsync();
            }
            catch(AggregateException ex)
            {
                Assert.IsTrue(ex.InnerException is ArgumentOutOfRangeException);
            }
        }

        [TestMethod]
        public async Task SeveralIntentHandlers()
        {
            var luisRecognizer = GetLuisRecognizer(verbose: true);

            var dialog = new IntentDialog("test", luisRecognizer)
                .Begin(async (step, cancellationToken) => { await step.Context.SendActivityAsync("begin"); return Dialog.EndOfTurn; })
                .Matches("Hi", async (step, cancellationToken) => { await step.Context.SendActivityAsync("hi"); return Dialog.EndOfTurn; })
                .Matches("Help", async (step, cancellationToken) => { await step.Context.SendActivityAsync("help"); return Dialog.EndOfTurn; })
                .Default(async (step, cancellationToken) => { await step.Context.SendActivityAsync("default"); return Dialog.EndOfTurn; });

            await GetTestFlow(dialog)
                .Send("hello")
                .AssertReply("begin")
                .Send("hello")
                .AssertReply("hi")
                .Send("help")
                .AssertReply("help")
                .Send("bye")
                .AssertReply("default")
                .StartTestAsync();
        }

        [TestMethod]
        public async Task CallChildDialog()
        {
            var luisRecognizer = GetLuisRecognizer(verbose: true);

            var child = new IntentDialog("child", luisRecognizer)
                .Begin(async (step, cancellationToken) => { await step.Context.SendActivityAsync($"child begin ({step.Options})"); return Dialog.EndOfTurn; })
                .Matches("Hi", async (step, cancellationToken) => { await step.Context.SendActivityAsync("child hi"); return await step.EndDialogAsync("result"); })
                .Default(async (step, cancellationToken) => { await step.Context.SendActivityAsync("child default"); return Dialog.EndOfTurn; });

            var parent = new IntentDialog("parent", luisRecognizer)
                .Begin(async (step, cancellationToken) => { await step.Context.SendActivityAsync("parent begin"); return Dialog.EndOfTurn; })
                .Matches("Hi", new WaterfallStep[]
                {
                    async (step, cancellationToken) => { return await step.BeginDialogAsync(child.Id, "options"); },
                    async (step, cancellationToken) => { await step.Context.SendActivityAsync($"parent hi ({step.Result})"); return Dialog.EndOfTurn; },
                })
                .Default(async (step, cancellationToken) => { await step.Context.SendActivityAsync("parent default"); return Dialog.EndOfTurn; });

            await GetTestFlow(new Dialog[] { parent, child }, parent.Id)
                .Send("hello")
                .AssertReply("parent begin")
                .Send("hello")
                .AssertReply("child begin (options)")
                .Send("hello")
                .AssertReply("child hi")
                .AssertReply("parent hi (result)")
                .Send("bye")
                .AssertReply("parent default")
                .StartTestAsync();
        }

        private IRecognizer GetLuisRecognizer(bool verbose = false, LuisPredictionOptions options = null)
        {
            var luisAppId = "ab48996d-abe2-4785-8eff-f18d15fc3560"; // Any value, it won't be used.
            var subscriptionKey = "cc7bbcc0-3715-44f0-b7c9-d8fee333dce1"; // Any value, it won't be used.
            var endpoint = "https://westus.api.cognitive.microsoft.com";
            var requestUrl = $"{endpoint}/luis/v2.0/apps/{luisAppId}";

            var messageHandler = new MockHttpMessageHandler();
            messageHandler.When(requestUrl).WithPartialContent("hello")
                .Respond("application/json", GetResponse("HiIntent.json"));
            messageHandler.When(requestUrl).WithPartialContent("bye")
                .Respond("application/json", GetResponse("ByeIntent.json"));
            messageHandler.When(requestUrl).WithPartialContent("cancel")
                .Respond("application/json", GetResponse("CancelIntent.json"));
            messageHandler.When(requestUrl).WithPartialContent("help")
                .Respond("application/json", GetResponse("HelpIntent.json"));

            var luisApp = new LuisApplication(luisAppId, subscriptionKey, endpoint);
            return new LuisRecognizer(luisApp, options, verbose, new MockedHttpClientHandler(messageHandler.ToHttpClient()));
        }

        private Stream GetResponse(string fileName)
            => File.OpenRead(Path.Combine(@"..\..\..\TestData\", fileName));

        private TestFlow GetTestFlow(Dialog dialog)
            => GetTestFlow(new Dialog[] { dialog }, dialog.Id);

        private TestFlow GetTestFlow(IEnumerable<Dialog> dialogs, string rootDialog)
        {
            var conversationState = new ConversationState(new MemoryStorage());

            var adapter = new TestAdapter()
                .Use(new AutoSaveStateMiddleware(conversationState));

            var dialogState = conversationState.CreateProperty<DialogState>(nameof(DialogState));

            var dialogSet = new DialogSet(dialogState);
            foreach (var dialog in dialogs)
            {
                dialogSet.Add(dialog);
            }

            return new TestFlow(adapter, async (turnContext, cancellationToken) =>
            {
                var dc = await dialogSet.CreateContextAsync(turnContext, cancellationToken);
                await dc.ContinueDialogAsync(cancellationToken);
                if (!turnContext.Responded)
                {
                    await dc.BeginDialogAsync(rootDialog, null, cancellationToken);
                }
            });
        }
    }
}
