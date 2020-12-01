const { expect } = require("chai");

describe("ElDoradoSavingAccounts", function() {
  it("Should allow owner to add a new provider", async function() {
    const signers = await ethers.getSigners();

    const ElDoradoSavingAccounts = await ethers.getContractFactory("ElDoradoSavingAccounts");
    const accounts = await ElDoradoSavingAccounts.deploy();
    
    await accounts.deployed();
    expect(await accounts.providersCount()).to.equal(0);

    const someAddress = await signers[0].getAddress() // TODO: Replace with an instance of a valid interface implementation
    await accounts.addProvider(someAddress);

    expect(await accounts.providersCount()).to.equal(1);
  });
});
