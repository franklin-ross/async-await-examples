it("await any thenable", async () => {
  const myAwaitable = {
    then(resolve, _reject) {
      resolve(5);
    }
  } as any;

  expect(await myAwaitable).toEqual(5);
});

it("thenables can reject", async () => {
  const myAwaitable = {
    then(_resolve, reject) {
      reject(new Error());
    }
  };

  expect(myAwaitable).rejects.toEqual(new Error());
});
