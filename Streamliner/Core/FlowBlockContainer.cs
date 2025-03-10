﻿using System;
using System.Collections.Generic;
using Streamliner.Blocks;
using Streamliner.Blocks.Base;

namespace Streamliner.Core
{
    internal class FlowBlockContainer
    {
        internal IEnumerable<BlockBase> Entrypoints => _entrypoints.Values;
        internal IEnumerable<BlockBase> Blocks => _blocks.Values;

        private readonly Dictionary<Guid, BlockBase> _entrypoints;
        private readonly Dictionary<Guid, BlockBase> _blocks;

        public FlowBlockContainer()
        {
            _entrypoints = new Dictionary<Guid, BlockBase>();
            _blocks = new Dictionary<Guid, BlockBase>();
        }

        internal void AddProducer<T>(ProducerBlock<T> producer)
        {
            _entrypoints.Add(producer.Header.BlockInfo.Id, producer);
            _blocks.Add(producer.Header.BlockInfo.Id, producer);
        }

        internal bool TryGetSourceBlock<T>(Guid id, out SourceBlockBase<T> block)
        {
            return TryGetBlock<SourceBlockBase<T>>(id, out block);
        }

        internal bool TryGetProducer<T>(Guid id, out ProducerBlock<T> block)
        {
            return TryGetBlock<ProducerBlock<T>>(id, out block);
        }

        internal bool TryGetConsumer<T>(Guid id, out ConsumerBlock<T> block)
        {
            return TryGetBlock<ConsumerBlock<T>>(id, out block);
        }

        internal bool TryGetTransformer<TIn, TOut>(Guid id, out TransformerBlock<TIn, TOut> block)
        {
            return TryGetBlock<TransformerBlock<TIn, TOut>>(id, out block);
        }

        internal void AddBlock(BlockBase block)
        {
            if (block.GetType().IsGenericType && block.GetType().GetGenericTypeDefinition() == typeof(ProducerBlock<>))
                throw new Exception($"Please use {nameof(AddProducer)} to add a producer block. {nameof(AddProducer)} is only for non producers.");

            _blocks.Add(block.Header.BlockInfo.Id, block);
        }

        internal bool TryGetBlock<T>(Guid blockId, out T block) where T : BlockBase
        {
            block = null;
            if (_blocks.TryGetValue(blockId, out BlockBase existingBlock))
                block = (T) existingBlock;

            return block != null;
        }
    }
}
