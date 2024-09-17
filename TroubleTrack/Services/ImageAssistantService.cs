using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using TroubleTrack.Utilities;

namespace TroubleTrack.Services
{
    internal class ImageAssistantService
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatCompletionService;
        private readonly IConfiguration _configuration;
        private readonly ChatHistory _chatHistory;

        public ImageAssistantService(IChatCompletionService chatCompletionService, Kernel kernel, IConfiguration configuration)
        {
            _chatCompletionService = chatCompletionService;
            _configuration = configuration;
            _kernel = kernel;
            _chatHistory = [];
            _chatHistory.AddSystemMessage(Prompts.SystemPrompt);
        }

        public async Task<ChatMessageContent> AnalyzeStepsAsync(List<string> Images, List<string> steps)
        {
            _chatHistory.AddUserMessage(steps.First());
            _chatHistory.AddUserMessage([new ImageContent(data: new(File.ReadAllBytes(Images.First())), "image/jpeg")]);
            var promptExecutionSettings = new OpenAIPromptExecutionSettings() { MaxTokens = 4096 };
            var result = await _chatCompletionService.GetChatMessageContentAsync(_chatHistory, promptExecutionSettings);
            return result;
        }
    }
}
