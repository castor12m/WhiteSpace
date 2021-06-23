// 비동기 호출 관련 1
// #UI#멈춤#버튼#클릭#태스크#비동기
// 문제점:
// UI 버튼 클릭 후, Wait 하는 함수를 비동기로 호출했음에도 불구하고 Main UI의 현재시간을 나타내는 Label이 멈춤(현재시간을 나타내는 Label은 Main Timer를 통해 업데이트 됨)

// 해결 전, 코드:
{
  private void btn_Click(object sender, EventArgs e)
  {
    var TaskProcess = Task<bool>.Run(() => ReturnCheck());
    if (TaskProcess.Result)
    {// ... //}
  }
  public async Task<bool> ReturnCheck()
  {
    while (bWait)
    {
       // ... //
       Task.Delay(50);
    }
  }
}

// 해결 후, 코드:
{
  private async void btn_Click(object sender, EventArgs e)
  {
    var TaskProcess = Task<bool>.Run(() => ReturnCheck());
    if (await Task.WhenAny(TaskProcess, Task.Delay(-1)) == TaskProcess)
    {
      if(TaskProcess.Result)
      {// ... //}
    }
  }

  public async Task<bool> ReturnCheck() {...}
}
 ​
 
// 비동기 호출 관련 2
// #태스크#비동기#콘솔#데드락
// 문제점:
// 콘솔에서는 비동기 함수가 데드락이 없지만 GUI 및 ASP.NET 응용프로그램에서는 데드락이 발생하는 이유.
// 아래의 코드가 콘솔 프로그램에서는 잘 동작하지만, GUI 및 ASP.NET 응용프로그램에서는 데드락 발생
{
  private static async Task DelayAsync()
  {
      await Task.Delay(1000);
  }

  // This method causes a deadlock when called in a GUI or ASP.NET context.
  public static void Test()
  {
      // Start the delay.
      var delayTask = DelayAsync();

      // Wait for the delay to complete.
      delayTask.Wait();
  }
}

// 이유 :
// (참고 Url : http://egloos.zum.com/sweeper/v/3193944)
// 콘솔 어플리케이션에서는 이것이 데드락을 유발하지 않음을 주목하라.
// 콘솔 어플리케이션은 한 번에 하나의 코드 묶음만 실행할 수 있는 SynchronizationContext 대신 Thread pool SynchronizationContext를 가진다.
// 그렇기에 await이 완료될 때, async 메쏘드의 나머지 부분은 thread pool thread에 스케쥴링된다.
// 이 부분은 프로그래머에게 혼란을 주는 데, 콘솔 어플리케이션에서 테스트할 때 문제 없던 코드를 GUI 또는 ASP.NET 어플리케이션으로 옮겨갈 때 데드락이 발생하기 때문이다.
