using FFImageLoading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portajel.Structures.Functional
{
    class ListHelper(IImageService imageService)
    {
        private IImageService _imageService = imageService;
        public async Task<bool> listPoller(double scroll, CancellationToken ct)
        {
            double previousScroll = 0;
            int stationaryCount = 0;
            bool isPaused = true;

            while (true)
            {
                try
                {
                    await Task.Delay(100, ct);
                    if (Math.Abs(scroll - previousScroll) < 0.1)
                    {
                        stationaryCount++;
                        if (stationaryCount >= 3 && isPaused)
                        {
                            await Task.Delay(200, ct);
                            _imageService.SetPauseWork(false);
                            isPaused = false;
                            // Trace.WriteLine("Scrolling stopped - resuming image loading :3");
                        }
                    }
                    else
                    {
                        stationaryCount = 0;
                        if (scroll > 200 && !isPaused)
                        {
                            _imageService.SetPauseWork(true);
                            isPaused = true;
                            // Trace.WriteLine("Fast scrolling detected - pausing image loading meow~");
                        }
                    }

                    previousScroll = scroll;
                    ct.ThrowIfCancellationRequested();
                }
                catch (Exception)
                {
                    // Trace.WriteLine("Album list polling cancelled");
                    break;
                }
            }
            return true;
        }
    }
}
