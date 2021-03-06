﻿using CommonLibraries.Extensions;
using CommonLibraries.Models.dbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommonLibraries.Models
{
    public class agvTaskCreator
    {
        private SQLCommunicator sqlComm = new SQLCommunicator();

        public agvTask CreateTask(Barcode barcode)
        {
            agvTask agvTask = null;
            if (barcode != null && barcode.HasData())
            {
                var model = sqlComm.GetModel();
                var currentStation = sqlComm.GetStation(barcode.Station);
                var destinationStation = sqlComm.GetStation(barcode.Destination);
                var result = GetResult(destinationStation);

                if (model != null && currentStation != null && destinationStation != null && !string.IsNullOrEmpty(result))
                {
                    agvTask = sqlComm.CreateTask(model, currentStation, destinationStation, result);
                    return agvTask;
                }
            }
            return agvTask;
        }

        private string GetResult(agvStation_Info destination)
        {
            var isLocked = destination.Status.Equals("Locked", StringComparison.OrdinalIgnoreCase);
            var isBusy = destination.Status.Equals("Busy", StringComparison.OrdinalIgnoreCase);

            if (!isLocked && !isBusy)
            {
                return "Ready";
            }

            return "In Queue";
        }
    }
}
