using Xunit;

namespace TestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void PushPop()
        {
            var sq = new Utils.StackQueue<int>();
            sq.Push(1);
            sq.Push(2);
            sq.Push(3);
            Assert.Equal(3, sq.Pop());
            Assert.Equal(2, sq.Pop());
            Assert.Equal(1, sq.Pop());
            Assert.Equal(0, sq.Pop());
            Assert.Equal(0, sq.Pop());
            Assert.Equal(0, sq.Pop());
        }
        [Fact]
        public void PushBottom()
        {
            var sq = new Utils.StackQueue<int>();
            sq.Push(1);
            sq.Push(2);
            sq.Push(3);
            Assert.Equal(1, sq.DequeueBottom());
            Assert.Equal(2, sq.DequeueBottom());
            Assert.Equal(3, sq.DequeueBottom());
            Assert.Equal(0, sq.DequeueBottom());
            Assert.Equal(0, sq.DequeueBottom());
            Assert.Equal(0, sq.DequeueBottom());
        }
        [Fact]
        public void PushMultipleTop()
        {
            var sq = new Utils.StackQueue<int>();
            sq.PushMultiple(new int[] { 1, 2, 3 });
            Assert.Equal(3, sq.Pop());
            Assert.Equal(2, sq.Pop());
            Assert.Equal(1, sq.Pop());
            Assert.Equal(0, sq.Pop());
            Assert.Equal(0, sq.Pop());
            Assert.Equal(0, sq.Pop());
        }
        [Fact]
        public void PeekTop()
        {
            var sq = new Utils.StackQueue<int>();
            sq.PushMultiple(new int[] { 1, 2, 3 });
            Assert.Equal(3, sq.Peek());
            Assert.Equal(3, sq.PeekTop());
            Assert.Equal(3, sq.PeekTop(0));
            Assert.Equal(2, sq.PeekTop(1));
            Assert.Equal(1, sq.PeekTop(2));
            Assert.Equal(0, sq.PeekTop(3));
            Assert.Equal(0, sq.PeekTop(4));
        }
    }
}
