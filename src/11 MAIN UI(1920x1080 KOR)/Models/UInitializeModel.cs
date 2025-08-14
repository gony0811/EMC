using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;


namespace EGGPLANT.Models
{
    /****************************************************************************/
    /* 프로그램 초기화 시 실행 및 종료 될 때 사용 될 객체들                     */
    /****************************************************************************/

    public sealed class Step
    {
        public string Title { get; }
        public Func<CancellationToken, Task> RunAsync { get; }

        public Step(string title, Func<CancellationToken, Task> run)
        {
            Title = title; RunAsync = run;
        }
    }

    public sealed class UiStatus
    {
        public string Title { get; private set; }
        public string Status { get; private set; }   // "진행 중...", "완료" 등
        public int Progress { get; private set; }    // 0~100

        public string ProgressLog { get; set; }
        public UiStatus(string title, string status, int progress)
        {
            Title = title;
            Status = status;
            Progress = progress;
        }
    }

    public sealed class StartupPipeline
    {
        private readonly IReadOnlyList<Step> _steps;
        private readonly IProgress<UiStatus> _ui;
        public StartupPipeline(IReadOnlyList<Step> steps, IProgress<UiStatus> ui)
            => (_steps, _ui) = (steps, ui);

        public async Task<bool> RunAsync(CancellationToken ct)
        {
            for (int i = 0; i < _steps.Count; i++)
            {
                var step = _steps[i];
                _ui.Report(new UiStatus(step.Title, "진행 중...", (int)(i * 100.0 / _steps.Count)));

                await step.RunAsync(ct).ConfigureAwait(false);

                _ui.Report(new UiStatus(step.Title, "완료", (int)((i + 1) * 100.0 / _steps.Count)));
            }
            return true;
        }
    }
}
