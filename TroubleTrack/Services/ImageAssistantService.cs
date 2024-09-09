using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace TroubleTrack.Services
{
    internal class ImageAssistantService
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatCompletionService;
        private readonly IConfiguration _configuration;
        private readonly string _systemPrompt;
        private readonly ChatHistory _chatHistory;

        public ImageAssistantService(IChatCompletionService chatCompletionService, Kernel kernel, IConfiguration configuration)
        {
            _chatCompletionService = chatCompletionService;
            _configuration = configuration;
            _kernel = kernel;
            _systemPrompt = _configuration.GetValue<string>("Prompts:SystemPrompt") ?? throw new InvalidOperationException("SystemPrompt is not configured.");
            _chatHistory = new ChatHistory();
            _chatHistory.AddSystemMessage(_systemPrompt);
        }

        public async Task<ChatMessageContent> AnalyzeStepsAsync(List<string> Images, List<string> steps)
        {
            _chatHistory.AddUserMessage(steps.First());
            _chatHistory.AddUserMessage([new ImageContent(data: new(File.ReadAllBytes(Images.First())), "image/jpeg")]);
            var result = await _chatCompletionService.GetChatMessageContentAsync(_chatHistory);
            return result;
        }
    }
}
