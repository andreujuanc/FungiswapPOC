
//SPDX-License-Identifier: UNLICENSED
pragma solidity ^0.5.15;

import "@openzeppelin-solidity/contracts/math/SafeMath.sol";
import "@openzeppelin-solidity/contracts/utils/ReentrancyGuard.sol";
import "@openzeppelin-solidity/contracts/utils/Address.sol";
import "@openzeppelin-solidity/contracts/token/ERC20/IERC20.sol";
import "@openzeppelin-solidity/contracts/GSN/Context.sol";
import "@openzeppelin-solidity/contracts/math/SafeMath.sol";
import "@openzeppelin-solidity/contracts/token/ERC20/IERC20.sol";
import "@openzeppelin-solidity/contracts/token/ERC20/ERC20Detailed.sol";
import "@openzeppelin-solidity/contracts/utils/EnumerableSet.sol";

import "@aave/aave-protocol/contracts/LendingPool.sol";
import "@aave/aave-protocol/contracts/LendingPoolCore.sol";
import "@aave/aave-protocol/contracts/LendingPoolDataProvider.sol";
import "@aave/aave-protocol/contracts/LendingPoolAddressesProvider.sol";
import "@aave/aave-protocol/contracts/LendingPoolParametersProvider.sol";
import "@aave/aave-protocol/contracts/LendingPoolLiquidationManager.sol";
import "@aaave/ave-protocol/contracts/libraries/EthAddressLib.sol";

import "../../IElDoradoSavingsProvider.sol";

contract AaveDeposits  is LendingPool, IElDoradoSavingsProvider {
   
    using SafeMath for uint256;
    using WadRayMath for uint256;
    using Address for address;

    LendingPoolAddressesProvider  public provider;

    LendingPool lendingPool lendingPool(provider.getLendingPool());
    
  
	mapping(address => uint256) private _address;

	
	mapping(address => uint256) private _balances;


	mapping(address => mapping(address => uint256)) private _allowances;


	uint256 private _totaldeposity;

    uint256  amount;

    uint256 _referral = 0;

    address daiAddress =  _address;

    IERC20(daiAddress).approve(provider.getLendingPoolCore(), amount);

     

   function deposit(address _address, uint256 amount, uint256 _referral) external view returns(uint256) {
        require(msg.value == amount);
        balanceOf[msg.sender] += amount;  
        lendingPool.deposit(_address, amount, _referral);
        
        sender = msg.sender;

       
    }
    
    
    
    function borrow(address _address, uint256 amount, uint256 _referral) external  view returns(uint256) {
        lendingPool.borrow(_address, amount, _referral);
        balanceOf[msg.sender] -= amount;
        msg.sender.transfer(amount);
        _address =msg.sender;
      
    }


    function getBalance() public view returns (uint256) {
        return address(this).balance;
    }
  
  
    
}