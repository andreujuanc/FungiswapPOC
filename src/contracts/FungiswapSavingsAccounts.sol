//SPDX-License-Identifier: UNLICENSED
pragma solidity >=0.5.15;

import "@nomiclabs/buidler/console.sol";

import "@openzeppelin/contracts/ownership/Ownable.sol";

import "@openzeppelin/contracts/utils/EnumerableSet.sol";


import "./IFungiswapProvider.sol";


contract ElDoradoSavingAccounts {
	using EnumerableSet for EnumerableSet.AddressSet;


	EnumerableSet.AddressSet private _providers;
	mapping(address => address) private _token_provider;
	mapping(address => uint256) private _balances;

	//mapping (address => mapping(address => address))  private _accounts_providers;

	constructor() public /*Ownable()*/
	{
		console.log("Deploying FungiswapSavingsAccounts Contract");
	}

	function providersCount() public view returns (uint256) {
		return _providers.length();
	}

	function getProviderByIndex(uint256 index) public view returns (address) {
		return _providers.get(index);
	}

	function getProviderByToken(address _tokenAddress) public view returns (address) {
		return _token_provider[_tokenAddress];
	}

	function addProvider(address providerAddress, address _tokenAddress) public {
		require(providerAddress != address(0), "Invalid provider address");
		require(_tokenAddress != address(0), "Invalid contract address");

		require(
			_token_provider[_tokenAddress] == address(0) || _token_provider[_tokenAddress] == providerAddress,
			"Another provider is already using that token"
		);

		// Callback for providers to implement internal erc20 approvals.
		IFungiswapSavingsProvider(providerAddress).approveToken(_tokenAddress);

		_token_provider[_tokenAddress] = providerAddress;
		_providers.add(providerAddress);
	}

	function depositAt(
		uint256 _providerIndex,
		address _tokenAddress,
		uint256 _amount
	) external returns (uint256) {
		return _deposit(msg.sender, getProviderByIndex(_providerIndex), _tokenAddress, _amount);
	}

	function depositOn(address _tokenAddress, uint256 _amount) external returns (uint256) {
		return _deposit(msg.sender, getProviderByToken(_tokenAddress), _tokenAddress, _amount);
	}

	//TODO: Change name to withdrawFrom
	function withdrawOn(address _tokenAddress, uint256 _amount) external returns (uint256) {
		return _withdraw(msg.sender, getProviderByToken(_tokenAddress), _tokenAddress, _amount);
	}

	function getBalance(address _tokenAddress) external view returns (uint256) {
		return _balanceOf(msg.sender, _tokenAddress);
	}

	function balanceOf(address account, address _tokenAddress) external view returns (uint256) {
		return _balanceOf(account, _tokenAddress);
	}

	function _balanceOf(address account, address _tokenAddress) private view returns (uint256) {
		return IFungiswapSavingsProvider(getProviderByToken(_tokenAddress)).getBalance(msg.sender, _tokenAddress);
	}

	function getEarnings(address _tokenAddress) external view returns (uint256) {
		//This wont work, because it might be diferent denominations
		uint256 total = 0;
		for (uint256 i = 0; i < _providers.length(); i++) {
			total += IFungiswapSavingsProvider(_providers.get(i)).getEarnings(msg.sender, _tokenAddress);
		}
		return total;
	}

	function _deposit(
		address account,
		address _providerAddress,
		address _tokenAddress,
		uint256 _amount
	) private returns (uint256) {
	
		uint256 result = IFungiswapSavingsProvider(_providerAddress).deposit(account, _tokenAddress, _amount);
		_balances[account] += result;
		
		return result;
	}

	function _withdraw(
		address account,
		address _providerAddress,
		address _tokenAddress,
		uint256 _amount
	) private returns (uint256) {
		uint256 result = IFungiswapSavingsProvider(_providerAddress).withdraw(account, _tokenAddress, _amount);
		_balances[account] -= result;
		return result;
	}
}
