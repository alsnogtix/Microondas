using Microsoft.AspNetCore.Mvc;

using SimuladorMicroondas.Models;

namespace SimuladorMicroondas.Controllers
{
    public class MicroondasController : Controller
    {
        private static int minutes = 0;
        private static int seconds = 0;
        private static bool isRunning = false;
        private static int potencia = 10;
        private static String aviso = "";
        private static Timer timer;

        public IActionResult Index()
        {
            ViewBag.Time = $"{minutes:D2}:{seconds:D2}";
            ViewBag.IsRunning = isRunning;
            ViewBag.Potencia = potencia;
            ViewBag.Aviso = aviso;
            return View();
        }

        public IActionResult Potencia()
        {
            if (!isRunning)
            {
                if (potencia < 10)
                {
                    potencia++;
                }
                else
                {
                    potencia = 1;
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Input(int number)
        {
            if (!isRunning)
            {
                if (seconds < 10 && minutes == 0)
                {
                    seconds = seconds * 10 + number;
                }else if ((seconds % 10) < 6 && minutes == 0)
                {
                    minutes = seconds / 10;
                    seconds = (seconds % 10) * 10 + number;
                    if (minutes >= 2)
                    {
                        minutes = 2;
                        seconds = 0;
                    }
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Start()
        {
            if (isRunning)
            {
                int saux = seconds;
                int maux = minutes;

                Stop();

                saux = saux + 30;
                if(saux > 59)
                {
                    maux = maux + 1;
                    if (maux >= 2)
                    {
                        maux = 2;
                        saux = 0;
                    }
                    else
                    {
                        saux = saux - 60;
                    }
                }
                seconds = saux;
                minutes = maux;
                isRunning = true;
                StartTimer();
                
            }else if (minutes > 0 || seconds > 0)
            {
                isRunning = true;
                StartTimer();
            }
            else
            {
                isRunning = true;
                seconds = 30;
                StartTimer();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Stop()
        {
            if (isRunning)
            {
                isRunning = false;
                StopTimer();
            }
            else
            {
                isRunning = false;
                StopTimer();
                minutes = 0;
                seconds = 0;
                potencia = 10;
            }
            

            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetTime()
        {
            return Json(new { minutes, seconds, isRunning, potencia, aviso });
        }

        private void StartTimer()
        {
            timer = new Timer(UpdateTimer, null, 0, 1000);
        }

        private void StopTimer()
        {
            timer?.Dispose();
        }

        private void UpdateTimer(object state)
        {
            if (seconds == 0)
            {
                if (minutes == 0)
                {
                    StopTimer();
                    isRunning = false;
                }
                else
                {
                    minutes--;
                    seconds = 59;
                }
            }
            else
            {
                seconds--;
            }

            
        }
    }
}
