import React from "react";
import  Login  from "./components/Login";
import  SignUp from "./components/SignUp"
import WithdrawDeposit from "./components/WithdrawDeposit";
import BuyStock from "./components/BuyStock";
import SellStock from "./components/SellStock";
import CreateStock from "./components/CreateStock";
import MarketSchedule from "./components/MarketSchedule";
import Market from "./components/Market";
import Portfolio from "./components/Portfolio";
import History from "./components/History";
import LimitOrders from "./components/LimitOrders"

const AppRoutes = [
  {
        index: true,
        element: <Market />
  },
  {
    path: '/funds',
    element: <WithdrawDeposit />
  },
  {
    path: '/buy-stock/:stock',
    element: <BuyStock />
  },
  {
    path: '/login',
    element: <Login />
  },
  {
    path: '/signup',
    element: <SignUp />
  },
  {
    path: '/sell-stock/:stock',
    element: <SellStock />
  },
  {
    path: '/create-stock',
    element: <CreateStock />
  },
  {
    path: '/change-schedule',
    element: <MarketSchedule />
  },
  {
    path: '/portfolio',
    element: <Portfolio />
  },
  {
    path: '/history',
    element: <History />
  },
  {
    path: '/limit-orders',
    element: <LimitOrders />
  }


];

export default AppRoutes;
