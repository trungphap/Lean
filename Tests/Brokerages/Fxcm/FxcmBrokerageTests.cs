﻿/*
 * QUANTCONNECT.COM - Democratizing Finance, Empowering Individuals.
 * Lean Algorithmic Trading Engine v2.0. Copyright 2014 QuantConnect Corporation.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
*/

using NUnit.Framework;
using QuantConnect.Brokerages.Fxcm;
using QuantConnect.Configuration;
using QuantConnect.Interfaces;
using QuantConnect.Securities;

namespace QuantConnect.Tests.Brokerages.Fxcm
{
    [TestFixture, Ignore("These tests require a configured and active FXCM practice account")]
    public partial class FxcmBrokerageTests : BrokerageTests
    {
        /// <summary>
        /// Creates the brokerage under test
        /// </summary>
        /// <returns>A connected brokerage instance</returns>
        protected override IBrokerage CreateBrokerage(IOrderProvider orderProvider, IHoldingsProvider holdingsProvider)
        {
            var server = Config.Get("fxcm-server");
            var terminal = Config.Get("fxcm-terminal");
            var userName = Config.Get("fxcm-user-name");
            var password = Config.Get("fxcm-password");
            var accountId = Config.Get("fxcm-account-id");

            return new FxcmBrokerage(orderProvider, holdingsProvider, server, terminal, userName, password, accountId);
        }

        /// <summary>
        /// Disposes of the brokerage and any external resources started in order to create it
        /// </summary>
        /// <param name="brokerage">The brokerage instance to be disposed of</param>
        protected override void DisposeBrokerage(IBrokerage brokerage)
        {
            brokerage.Disconnect();
        }

        /// <summary>
        /// Provides the data required to test each order type in various cases
        /// </summary>
        public override TestCaseData[] OrderParameters
        {
            get
            {
                return new[]
                {
                    new TestCaseData(new MarketOrderTestParameters(Symbol, SecurityType)).SetName("MarketOrder"),
                    new TestCaseData(new FxcmLimitOrderTestParameters(Symbol, SecurityType, HighPrice, LowPrice)).SetName("LimitOrder"),
                    new TestCaseData(new FxcmStopMarketOrderTestParameters(Symbol, SecurityType, HighPrice, LowPrice)).SetName("StopMarketOrder"),
                };
            }
        }

        /// <summary>
        /// Gets the symbol to be traded, must be shortable
        /// </summary>
        protected override string Symbol
        {
            get { return "EURUSD"; }
        }

        /// <summary>
        /// Gets the security type associated with the <see cref="BrokerageTests.Symbol"/>
        /// </summary>
        protected override SecurityType SecurityType
        {
            get { return SecurityType.Forex; }
        }

        /// <summary>
        /// Gets a high price for the specified symbol so a limit sell won't fill
        /// </summary>
        protected override decimal HighPrice
        {
            // FXCM requires order prices to be not more than 5600 pips from the market price (at least for EURUSD)
            get { return 1.5m; }
        }

        /// <summary>
        /// Gets a low price for the specified symbol so a limit buy won't fill
        /// </summary>
        protected override decimal LowPrice
        {
            // FXCM requires order prices to be not more than 5600 pips from the market price (at least for EURUSD)
            get { return 0.7m; }
        }

        /// <summary>
        /// Gets the current market price of the specified security
        /// </summary>
        protected override decimal GetAskPrice(string symbol, SecurityType securityType)
        {
            // not used, we use bid/ask prices
            return 0;
        }

        /// <summary>
        /// Gets the default order quantity
        /// </summary>
        protected override int GetDefaultQuantity()
        {
            // FXCM requires a multiple of 1000 for Forex instruments
            return 1000;
        }
    }
}