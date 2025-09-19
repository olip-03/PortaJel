using FFImageLoading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Structures.Functional
{
    // class ListHelper(IImageService imageService, CancellationToken token)
    // {
    //     private IImageService _imageService = imageService;
    //     public void ListPoller(ref double scroll, CancellationToken ct)
    //     {
    //         double previousScroll = 0;
    //         int stationaryCount = 0;
    //         bool isPaused = true;
    //         
    //         _ = Task.Run(async () =>
    //         {
    //             while (true)
    //             {
    //                 try
    //                 {
    //                     await Task.Delay(100, ct);
    //                     if (Math.Abs(scroll - previousScroll) < 0.1)
    //                     {
    //                         stationaryCount++;
    //                         if (stationaryCount >= 3 && isPaused)
    //                         {
    //                             await Task.Delay(200, ct);
    //                             _imageService.SetPauseWork(false);
    //                             isPaused = false;
    //                             // Trace.WriteLine("Scrolling stopped - resuming image loading :3");
    //                         }
    //                     }
    //                     else
    //                     {
    //                         stationaryCount = 0;
    //                         if (scroll > 200 && !isPaused)
    //                         {
    //                             _imageService.SetPauseWork(true);
    //                             isPaused = true;
    //                             // Trace.WriteLine("Fast scrolling detected - pausing image loading meow~");
    //                         }
    //                     }
    //
    //                     previousScroll = scroll;
    //                     ct.ThrowIfCancellationRequested();
    //                 }
    //                 catch (Exception)
    //                 {
    //                     // Trace.WriteLine("Album list polling cancelled");
    //                     break;
    //                 }
    //             }
    //         }, ct);
    //         return true;
    //     }
    // }
}
